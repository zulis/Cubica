using System;
using System.Collections.Generic;
using System.Linq;
using ComponentFramework.Core;
using ComponentFramework.Structures;
using ComponentFramework.Tools;
using MTV3D65;

namespace ComponentFramework.Components
{
    public class Keyboard : Component, IKeyboardService
    {
        const int SimultaneousKeys = 256; // Overkill, but prevents overflows
        readonly TV_KEYDATA[] keyBuffer = new TV_KEYDATA[SimultaneousKeys];

        // Using TimedButtonState's here would be more complex... and unnecessary?
        readonly Dictionary<CONST_TV_KEY, TVButtonState> keyStates = new Dictionary<CONST_TV_KEY, TVButtonState>(EnumComparer<CONST_TV_KEY>.Instance);

        public Keyboard(ICore core) : base(core) { }

        public TVButtonState GetKeyState(CONST_TV_KEY key)
        {
            TVButtonState state;
            if (!keyStates.TryGetValue(key, out state))
                state = TVButtonState.Up;

            return state;
        }
        
        public CONST_TV_KEY GetTVKey(string key)
        {
            switch (key.Trim().ToUpper())
            {
                case "0":
                    return CONST_TV_KEY.TV_KEY_0;
                case "1":
                    return CONST_TV_KEY.TV_KEY_1;
                case "2":
                    return CONST_TV_KEY.TV_KEY_2;
                case "3":
                    return CONST_TV_KEY.TV_KEY_3;
                case "4":
                    return CONST_TV_KEY.TV_KEY_4;
                case "5":
                    return CONST_TV_KEY.TV_KEY_5;
                case "6":
                    return CONST_TV_KEY.TV_KEY_6;
                case "7":
                    return CONST_TV_KEY.TV_KEY_7;
                case "8":
                    return CONST_TV_KEY.TV_KEY_8;
                case "9":
                    return CONST_TV_KEY.TV_KEY_9;
                case "A":
                    return CONST_TV_KEY.TV_KEY_A;
                case "ABNT_C1":
                    return CONST_TV_KEY.TV_KEY_ABNT_C1;
                case "ABNT_C2":
                    return CONST_TV_KEY.TV_KEY_ABNT_C2;
                case "ADD":
                    return CONST_TV_KEY.TV_KEY_ADD;
                case "ALT_LEFT":
                    return CONST_TV_KEY.TV_KEY_ALT_LEFT;
                case "ALT_RIGHT":
                    return CONST_TV_KEY.TV_KEY_ALT_RIGHT;
                case "APOSTROPHE":
                    return CONST_TV_KEY.TV_KEY_APOSTROPHE;
                case "APPS":
                    return CONST_TV_KEY.TV_KEY_APPS;
                case "AT":
                    return CONST_TV_KEY.TV_KEY_AT;
                case "AX":
                    return CONST_TV_KEY.TV_KEY_AX;
                case "B":
                    return CONST_TV_KEY.TV_KEY_B;
                case "BACKSLASH":
                    return CONST_TV_KEY.TV_KEY_BACKSLASH;
                case "BACKSPACE":
                    return CONST_TV_KEY.TV_KEY_BACKSPACE;
                case "C":
                    return CONST_TV_KEY.TV_KEY_C;
                case "CALCULATOR": 
                    return CONST_TV_KEY.TV_KEY_CALCULATOR;
                case "CAPITAL":
                    return CONST_TV_KEY.TV_KEY_CAPITAL;
                case "CAPSLOCK":
                    return CONST_TV_KEY.TV_KEY_CAPSLOCK;
                case "CIRCUMFLEX":
                    return CONST_TV_KEY.TV_KEY_CIRCUMFLEX;
                case "COLON":
                    return CONST_TV_KEY.TV_KEY_COLON;
                case "COMMA":
                    return CONST_TV_KEY.TV_KEY_COMMA;
                case "CONVERT":
                    return CONST_TV_KEY.TV_KEY_CONVERT;
                case "D": return CONST_TV_KEY.TV_KEY_D;
                case "DECIMAL":
                    return CONST_TV_KEY.TV_KEY_DECIMAL;
                case "DELETE":
                    return CONST_TV_KEY.TV_KEY_DELETE;
                case "DIVIDE":
                    return CONST_TV_KEY.TV_KEY_DIVIDE;
                case "DOWN":
                    return CONST_TV_KEY.TV_KEY_DOWN;
                case "DOWNARROW":
                    return CONST_TV_KEY.TV_KEY_DOWNARROW;
                case "E":
                    return CONST_TV_KEY.TV_KEY_E;
                case "END":
                    return CONST_TV_KEY.TV_KEY_END;
                case "EQUALS":
                    return CONST_TV_KEY.TV_KEY_EQUALS;
                case "ESCAPE":
                    return CONST_TV_KEY.TV_KEY_ESCAPE;
                case "F":
                    return CONST_TV_KEY.TV_KEY_F;
                case "F1":
                    return CONST_TV_KEY.TV_KEY_F1;
                case "F10":
                    return CONST_TV_KEY.TV_KEY_F10;
                case "F11":
                    return CONST_TV_KEY.TV_KEY_F11;
                case "F12":
                    return CONST_TV_KEY.TV_KEY_F12;
                case "F13":
                    return CONST_TV_KEY.TV_KEY_F13;
                case "F14":
                    return CONST_TV_KEY.TV_KEY_F14;
                case "F15":
                    return CONST_TV_KEY.TV_KEY_F15;
                case "F2":
                    return CONST_TV_KEY.TV_KEY_F2;
                case "F3":
                    return CONST_TV_KEY.TV_KEY_F3;
                case "F4":
                    return CONST_TV_KEY.TV_KEY_F4;
                case "F5":
                    return CONST_TV_KEY.TV_KEY_F5;
                case "F6":
                    return CONST_TV_KEY.TV_KEY_F6;
                case "F7":
                    return CONST_TV_KEY.TV_KEY_F7;
                case "F8":
                    return CONST_TV_KEY.TV_KEY_F8;
                case "F9":
                    return CONST_TV_KEY.TV_KEY_F9;
                case "G":
                    return CONST_TV_KEY.TV_KEY_G;
                case "GRAVE":
                    return CONST_TV_KEY.TV_KEY_GRAVE;
                case "H":
                    return CONST_TV_KEY.TV_KEY_H;
                case "HOME":
                    return CONST_TV_KEY.TV_KEY_HOME;
                case "I":
                    return CONST_TV_KEY.TV_KEY_I;
                case "INSERT":
                    return CONST_TV_KEY.TV_KEY_INSERT;
                case "J":
                    return CONST_TV_KEY.TV_KEY_J;
                case "K":
                    return CONST_TV_KEY.TV_KEY_K;
                case "KANA":
                    return CONST_TV_KEY.TV_KEY_KANA;
                case "KANJI":
                    return CONST_TV_KEY.TV_KEY_KANJI;
                case "L":
                    return CONST_TV_KEY.TV_KEY_L;
                case "LEFT":
                    return CONST_TV_KEY.TV_KEY_LEFT;
                case "LEFTARROW":
                    return CONST_TV_KEY.TV_KEY_LEFTARROW;
                case "LEFTBRACKET":
                    return CONST_TV_KEY.TV_KEY_LEFTBRACKET;
                case "LEFTCONTROL":
                    return CONST_TV_KEY.TV_KEY_LEFTCONTROL;
                case "LEFTMENU":
                    return CONST_TV_KEY.TV_KEY_LEFTMENU;
                case "LEFTSHIFT":
                    return CONST_TV_KEY.TV_KEY_LEFTSHIFT;
                case "LEFTWINDOWS":
                    return CONST_TV_KEY.TV_KEY_LEFTWINDOWS;
                case "M":
                    return CONST_TV_KEY.TV_KEY_M;
                case "MAIL":
                    return CONST_TV_KEY.TV_KEY_MAIL;
                case "MEDIASELECT":
                    return CONST_TV_KEY.TV_KEY_MEDIASELECT;
                case "MEDIASTOP":
                    return CONST_TV_KEY.TV_KEY_MEDIASTOP;
                case "MINUS":
                    return CONST_TV_KEY.TV_KEY_MINUS;
                case "MULTIPLY":
                    return CONST_TV_KEY.TV_KEY_MULTIPLY;
                case "MUTE":
                    return CONST_TV_KEY.TV_KEY_MUTE;
                case "MYCOMPUTER":
                    return CONST_TV_KEY.TV_KEY_MYCOMPUTER;
                case "N":
                    return CONST_TV_KEY.TV_KEY_N;
                case "NEXT":
                    return CONST_TV_KEY.TV_KEY_NEXT;
                case "NEXTTRACK":
                    return CONST_TV_KEY.TV_KEY_NEXTTRACK;
                case "NOCONVERT":
                    return CONST_TV_KEY.TV_KEY_NOCONVERT;
                case "NUMLOCK":
                    return CONST_TV_KEY.TV_KEY_NUMLOCK;
                case "NUMPAD0":
                    return CONST_TV_KEY.TV_KEY_NUMPAD0;
                case "NUMPAD1":
                    return CONST_TV_KEY.TV_KEY_NUMPAD1;
                case "NUMPAD2":
                    return CONST_TV_KEY.TV_KEY_NUMPAD2;
                case "NUMPAD3":
                    return CONST_TV_KEY.TV_KEY_NUMPAD3;
                case "NUMPAD4":
                    return CONST_TV_KEY.TV_KEY_NUMPAD4;
                case "NUMPAD5":
                    return CONST_TV_KEY.TV_KEY_NUMPAD5;
                case "NUMPAD6":
                    return CONST_TV_KEY.TV_KEY_NUMPAD6;
                case "NUMPAD7":
                    return CONST_TV_KEY.TV_KEY_NUMPAD7;
                case "NUMPAD8":
                    return CONST_TV_KEY.TV_KEY_NUMPAD8;
                case "NUMPAD9":
                    return CONST_TV_KEY.TV_KEY_NUMPAD9;
                case "NUMPADCOMMA":
                    return CONST_TV_KEY.TV_KEY_NUMPADCOMMA;
                case "NUMPADENTER":
                    return CONST_TV_KEY.TV_KEY_NUMPADENTER;
                case "NUMPADEQUALS":
                    return CONST_TV_KEY.TV_KEY_NUMPADEQUALS;
                case "NUMPADMINUS":
                    return CONST_TV_KEY.TV_KEY_NUMPADMINUS;
                case "NUMPADPERIOD":
                    return CONST_TV_KEY.TV_KEY_NUMPADPERIOD;
                case "NUMPADPLUS":
                    return CONST_TV_KEY.TV_KEY_NUMPADPLUS;
                case "NUMPADSLASH":
                    return CONST_TV_KEY.TV_KEY_NUMPADSLASH;
                case "NUMPADSTAR":
                    return CONST_TV_KEY.TV_KEY_NUMPADSTAR;
                case "O":
                    return CONST_TV_KEY.TV_KEY_O;
                case "OEM_102":
                    return CONST_TV_KEY.TV_KEY_OEM_102;
                case "P":
                    return CONST_TV_KEY.TV_KEY_P;
                case "PAGEDOWN":
                    return CONST_TV_KEY.TV_KEY_PAGEDOWN;
                case "PAGEUP":
                    return CONST_TV_KEY.TV_KEY_PAGEUP;
                case "PAUSE":
                    return CONST_TV_KEY.TV_KEY_PAUSE;
                case "PERIOD":
                    return CONST_TV_KEY.TV_KEY_PERIOD;
                case "PLAYPAUSE":
                    return CONST_TV_KEY.TV_KEY_PLAYPAUSE;
                case "POWER":
                    return CONST_TV_KEY.TV_KEY_POWER;
                case "PREVTRACK":
                    return CONST_TV_KEY.TV_KEY_PREVTRACK;
                case "PRIOR":
                    return CONST_TV_KEY.TV_KEY_PRIOR;
                case "Q":
                    return CONST_TV_KEY.TV_KEY_Q;
                case "R":
                    return CONST_TV_KEY.TV_KEY_R;
                case "RETURN":
                    return CONST_TV_KEY.TV_KEY_RETURN;
                case "RIGHT":
                    return CONST_TV_KEY.TV_KEY_RIGHT;
                case "RIGHTARROW":
                    return CONST_TV_KEY.TV_KEY_RIGHTARROW;
                case "RIGHTBRACKET":
                    return CONST_TV_KEY.TV_KEY_RIGHTBRACKET;
                case "RIGHTCONTROL":
                    return CONST_TV_KEY.TV_KEY_RIGHTCONTROL;
                case "RIGHTMENU":
                    return CONST_TV_KEY.TV_KEY_RIGHTMENU;
                case "RIGHTSHIFT":
                    return CONST_TV_KEY.TV_KEY_RIGHTSHIFT;
                case "RWIN":
                    return CONST_TV_KEY.TV_KEY_RWIN;
                case "S":
                    return CONST_TV_KEY.TV_KEY_S;
                case "SCROLL":
                    return CONST_TV_KEY.TV_KEY_SCROLL;
                case "SEMICOLON":
                    return CONST_TV_KEY.TV_KEY_SEMICOLON;
                case "SLASH":
                    return CONST_TV_KEY.TV_KEY_SLASH;
                case "SLEEP":
                    return CONST_TV_KEY.TV_KEY_SLEEP;
                case "SPACE":
                    return CONST_TV_KEY.TV_KEY_SPACE;
                case "STOP":
                    return CONST_TV_KEY.TV_KEY_STOP;
                case "SUBTRACT":
                    return CONST_TV_KEY.TV_KEY_SUBTRACT;
                case "SYSRQ":
                    return CONST_TV_KEY.TV_KEY_SYSRQ;
                case "T":
                    return CONST_TV_KEY.TV_KEY_T;
                case "TAB":
                    return CONST_TV_KEY.TV_KEY_TAB;
                case "U":
                    return CONST_TV_KEY.TV_KEY_U;
                case "UNDERLINE":
                    return CONST_TV_KEY.TV_KEY_UNDERLINE;
                case "UNLABELED":
                    return CONST_TV_KEY.TV_KEY_UNLABELED;
                case "UP":
                    return CONST_TV_KEY.TV_KEY_UP;
                case "UPARROW":
                    return CONST_TV_KEY.TV_KEY_UPARROW;
                case "V":
                    return CONST_TV_KEY.TV_KEY_V;
                case "VOLUMEDOWN":
                    return CONST_TV_KEY.TV_KEY_VOLUMEDOWN;
                case "VOLUMEUP":
                    return CONST_TV_KEY.TV_KEY_VOLUMEUP;
                case "W":
                    return CONST_TV_KEY.TV_KEY_W;
                case "WAKE":
                    return CONST_TV_KEY.TV_KEY_WAKE;
                case "WEBBACK":
                    return CONST_TV_KEY.TV_KEY_WEBBACK;
                case "WEBFAVORITES":
                    return CONST_TV_KEY.TV_KEY_WEBFAVORITES;
                case "WEBFORWARD":
                    return CONST_TV_KEY.TV_KEY_WEBFORWARD;
                case "WEBHOME":
                    return CONST_TV_KEY.TV_KEY_WEBHOME;
                case "WEBREFRESH":
                    return CONST_TV_KEY.TV_KEY_WEBREFRESH;
                case "WEBSEARCH":
                    return CONST_TV_KEY.TV_KEY_WEBSEARCH;
                case "WEBSTOP":
                    return CONST_TV_KEY.TV_KEY_WEBSTOP;
                case "X":
                    return CONST_TV_KEY.TV_KEY_X;
                case "Y":
                    return CONST_TV_KEY.TV_KEY_Y;
                case "YEN":
                    return CONST_TV_KEY.TV_KEY_YEN;
                case "Z":
                    return CONST_TV_KEY.TV_KEY_Z;
                default:
                    // We must return something, but we will not get in here.
                    return CONST_TV_KEY.TV_KEY_POWER;
            }
        }

        public IEnumerable<CONST_TV_KEY> Down
        {
            get { return from state in keyStates where state.Value == TVButtonState.Down select state.Key; }
        }

        public IEnumerable<CONST_TV_KEY> Pressed
        {
            get { return from state in keyStates where state.Value == TVButtonState.Pressed select state.Key; }
        }

        public IEnumerable<CONST_TV_KEY> Released
        {
            get { return from state in keyStates where state.Value == TVButtonState.Released select state.Key; }
        }

        readonly List<CONST_TV_KEY> toRemove = new List<CONST_TV_KEY>();
        public override void Update(TimeSpan elapsedTime)
        {
            if (keyStates.Keys.Count > 0)
            {
                foreach (var key in keyStates.Keys)
                    switch (keyStates[key])
                    {
                        case TVButtonState.Pressed:
                        case TVButtonState.Released:
                            toRemove.Add(key);
                            break;
                    }

                if (toRemove.Count > 0)
                {
                    foreach (var key in toRemove)
                    {
                        if (keyStates[key] == TVButtonState.Pressed)
                            keyStates[key] = TVButtonState.Down;
                        else
                            keyStates.Remove(key);
                    }
                    toRemove.Clear();
                }
            }

            int keyCount = 0;
            InputEngine.GetKeyBuffer(keyBuffer, ref keyCount);
            for (int i = 0; i < keyCount; i++)
            {
                var keyData = keyBuffer[i];
                var key = (CONST_TV_KEY)keyBuffer[i].Key;
                var state = keyData.Pressed == 1 ? TVButtonState.Pressed : keyData.Released == 1 ? TVButtonState.Released : TVButtonState.Up;

                keyStates.Remove(key);
                if (state != TVButtonState.Up)
                    keyStates.Add(key, state);
            }
        }
    }

    public interface IKeyboardService : IService
    {
        TVButtonState GetKeyState(CONST_TV_KEY key);
        CONST_TV_KEY GetTVKey(string key);

        IEnumerable<CONST_TV_KEY> Down { get; }
        IEnumerable<CONST_TV_KEY> Pressed { get; }
        IEnumerable<CONST_TV_KEY> Released { get; }
    }
}