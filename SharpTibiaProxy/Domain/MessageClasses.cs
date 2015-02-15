using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpTibiaProxy.Domain
{
    public enum MessageClasses
    {
        NONE = 0x00,

        SPEAK_SAY = 0x01,
        SPEAK_WHISPER = 0x02,
        SPEAK_YELL = 0x03,
        PRIVATE_FROM = 0x04,
        PRIVATE_TO = 0x05,
        CHANNEL_MANAGEMENT = 0x06,
        CHANNEL = 0x07,
        CHANNEL_HIGHLIGHT = 0x08,
        SPEAK_SPELL = 0x09,
        NPC_FROM_START_BLOCK = 0x0A,
        NPC_FROM = 0x0B,
        NPC_TO = 0x0C,
        GAMEMASTER_BROADCAST = 0x0D,
        GAMEMASTER_CHANNEL = 0x0E,
        GAMEMASTER_PRIVATE_FROM = 0x0F,
        GAMEMASTER_PRIVATE_TO = 0x10,
        SPEAK_MONSTER_SAY = 0x24,
        SPEAK_MONSTER_YELL = 0x25,

        SPEAK_FIRST = SPEAK_SAY,
        SPEAK_LAST = GAMEMASTER_PRIVATE_TO,
        SPEAK_MONSTER_FIRST = SPEAK_MONSTER_SAY,
        SPEAK_MONSTER_LAST = SPEAK_MONSTER_YELL,

        STATUS_CONSOLE_BLUE = 0x04, /*Teal message in local chat*/
        STATUS_CONSOLE_RED = 0x0D, /*Red message in console*/
        STATUS_DEFAULT = 0x11, /*White message at the bottom of the game window and in the console*/
        STATUS_WARNING = 0x12, /*Red message in game window and in the console*/
        EVENT_ADVANCE = 0x13, /*White message in game window and in the console*/
        GAME_HIGHLIGHT = 0x14, //10.53
        STATUS_SMALL = 0x15, /*White message at the bottom of the game window"*/
        INFO_DESCR = 0x16, /*Green message in game window and in the console*/
        DAMAGE_DEALT = 0x17,
        DAMAGE_RECEIVED = 0x18,
        HEALED = 0x19,
        EXPERIENCE = 0x1A,
        DAMAGE_OTHERS = 0x1B,
        HEALED_OTHERS = 0x1C,
        EXPERIENCE_OTHERS = 0x1D,
        EVENT_DEFAULT = 0x1E, /*White message at the bottom of the game window and in the console*/
        LOOT = 0x1F, /*Green message in game window and in the console*/
        TRADE_NPC = 0x20, /*Green message in game window and in the console*/
        EVENT_GUILD = 0x21, /*Green message in game window and in the console*/
        PARTY_MANAGEMENT = 0x22, /*Green message in game window and in the console*/
        PARTY = 0x23, /*Green message in game window and in the console*/
        EVENT_ORANGE = 0x24, /*Orange message in local chat*/
        STATUS_CONSOLE_ORANGE = 0x25, /*Orange message in local chat*/
        REPORT = 0x26, /*White message in game window and in the console*/
        HOTKEY_USE = 0x27, /*Green message in game window*/
        TUTORIAL_HINT = 0x28,
        THANK_YOU = 0x29,
        MARKET = 0x30,
        BEYOND_LAST = 0x31
    };
}
