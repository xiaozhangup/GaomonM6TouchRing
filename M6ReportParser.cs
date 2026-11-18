using System;
using OpenTabletDriver.Configurations.Parsers.UCLogic;
using OpenTabletDriver.Plugin.Tablet;
using OpenTabletDriver.Plugin.Tablet.Wheel;

namespace GaomonM6TouchRing;

public sealed class M6ReportParser : IReportParser<IDeviceReport>
{
    private const int HardwareButtonCount = 13;
    private const int SoftKeyCount = 16;
    private const int TabletWidth = 50800;
    private const int SoftKeyStripHeight = 1000; // 5 mm at 5080 LPI

    private readonly UCLogicV2ReportParser _baseParser = new();
    private bool[] _hardwareButtons = new bool[HardwareButtonCount];
    private int? _activeSoftKey;
    private bool _tipDown;

    public IDeviceReport Parse(byte[] data)
    {
        if (data.Length != 12)
            return new DeviceReport(data);

        if (data[1] == 0xf0)
        {
            // PR #4984: zero means release; raw positions decrease clockwise.
            byte position = data[5];
            if (position > 12)
                return new DeviceReport(data);

            return new RingReport(data, position == 0 ? null : (uint)(12 - position));
        }

        IDeviceReport report = _baseParser.Parse(data);
        if (report is IAuxReport aux)
        {
            _hardwareButtons = aux.AuxButtons;
            return GetAuxReport(data);
        }

        // The tip switch is bit 0; the first contact packet can have zero pressure.
        bool tipDown = (data[1] & 1) != 0;
        if (_activeSoftKey.HasValue)
        {
            if (tipDown)
                return new DeviceReport(data);

            _activeSoftKey = null;
            _tipDown = false;
            return GetAuxReport(data);
        }

        // Decide on touchdown so a stroke that started below the strip remains a stroke.
        if (tipDown && !_tipDown && report is ITabletReport pen
            && pen.Position.Y < SoftKeyStripHeight && pen.Position.X <= TabletWidth)
        {
            _activeSoftKey = Math.Min((int)pen.Position.X * SoftKeyCount / TabletWidth, SoftKeyCount - 1);
            _tipDown = true;
            return GetAuxReport(data);
        }

        _tipDown = tipDown;
        return report;
    }

    private IAuxReport GetAuxReport(byte[] data)
    {
        var buttons = new bool[HardwareButtonCount + SoftKeyCount];
        Array.Copy(_hardwareButtons, buttons, HardwareButtonCount);
        if (_activeSoftKey is int key)
            buttons[HardwareButtonCount + key] = true;

        return new ButtonReport(data, buttons);
    }

    private sealed class ButtonReport(byte[] raw, bool[] buttons) : IAuxReport
    {
        public byte[] Raw { get; set; } = raw;
        public bool[] AuxButtons { get; set; } = buttons;
    }

    private sealed class RingReport(byte[] raw, uint? position) : IAbsoluteWheelReport
    {
        public byte[] Raw { get; set; } = raw;
        public uint?[] AnalogPositions { get; set; } = [position];
    }
}
