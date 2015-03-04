using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using SharpTibiaProxy.Domain;

namespace SharpTibiaProxy.Util
{
    public class MemoryAddresses
    {
        public readonly long ClientRsa;

        public readonly long ClientServerStart;
        public readonly long ClientServerEnd;
        public readonly long ClientServerStep;
        public readonly long ClientServerDistanceHostname;
        public readonly long ClientServerDistanceIP;
        public readonly long ClientServerDistancePort;
        public readonly long ClientServerMax;
        public readonly long ClientSelectedCharacter;

        public readonly long ClientProxyCheckFunctionPointer;
        public readonly byte[] ClientProxyCheckFunctionOriginal = new byte[] { 0x8B, 0x40, 0x20, 0x80, 0x38, 0x00, 0x74, 0x18, 0x80, 0x78, 0x01, 0x00, 0x74, 0x12, 0x80 };
        public readonly byte[] ClientProxyCheckFunctionNOP = new byte[] { 0x8B, 0x40, 0x20, 0x83, 0x38, 0x00, 0xB8, 0x00, 0x00, 0x00, 0x00, 0x0F, 0x95, 0xD0, 0xC3 };

        public readonly long ClientMultiClient;
        public readonly byte ClientMultiClientJMP = 0xEB;
        public readonly byte ClientMultiClientJNZ = 0x75;

        public readonly long ClientStatus;

        public readonly long PlayerGoX;
        public readonly long PlayerGoY;
        public readonly long PlayerGoZ;

        public readonly long ClientBattleListStart;
        public readonly int ClientBattleListStep = 176;
        public readonly int ClientBattleListMaxCreatures = 1300;
        public readonly int ClientBattleListCreatureWalkDistance = 80;

        public MemoryAddresses(Client client)
        {
            if (client.Version == ClientVersion.Version961)
            {
                ClientRsa = client.BaseAddress + 0x320D40;
                ClientServerStart = client.BaseAddress + 0x3B06D0;
                ClientServerStep = 112;
                ClientServerMax = 10;
                ClientServerDistancePort = 100;
                ClientSelectedCharacter = client.BaseAddress + 0x3B9EF4;
                ClientMultiClient = client.BaseAddress + 0x12B387;

                ClientStatus = client.BaseAddress + 0x3B9EA8;

                PlayerGoX = 0;
                PlayerGoY = 0;
                PlayerGoZ = 0;

                ClientBattleListStart = 0;
            }
            else if (client.Version == ClientVersion.Version963)
            {
                ClientRsa = client.BaseAddress + 0x324EC0;
                ClientServerStart = client.BaseAddress + 0x3B34F8;
                ClientServerStep = 112;
                ClientServerMax = 10;
                ClientServerDistancePort = 100;
                ClientSelectedCharacter = client.BaseAddress + 0x3BCD10;
                ClientMultiClient = client.BaseAddress + 0x12E807;

                ClientStatus = client.BaseAddress + 0x3BCCC4;

                PlayerGoX = client.BaseAddress + 0x57FEA0;
                PlayerGoY = client.BaseAddress + 0x57FE98;
                PlayerGoZ = client.BaseAddress + 0x548004;

                ClientBattleListStart = client.BaseAddress + 0x548008;
            }
            else if (client.Version == ClientVersion.Version970)
            {
                ClientRsa = client.BaseAddress + 0x324EC0;
                ClientServerStart = client.BaseAddress + 0x3B34F8;
                ClientServerStep = 112;
                ClientServerMax = 10;
                ClientServerDistancePort = 100;
                ClientSelectedCharacter = client.BaseAddress + 0x3BCD10;
                ClientMultiClient = client.BaseAddress + 0x12EC57;

                ClientStatus = client.BaseAddress + 0x3BCCC4;

                PlayerGoX = client.BaseAddress + 0x57FEA0;
                PlayerGoY = client.BaseAddress + 0x57FE98;
                PlayerGoZ = client.BaseAddress + 0x548004;

                ClientBattleListStart = client.BaseAddress + 0x548008;
            }
            else if (client.Version == ClientVersion.Version981)
            {
                ClientRsa = client.BaseAddress + 0x327FB8;
                ClientServerStart = client.BaseAddress + 0x3B7558;
                ClientServerStep = 112;
                ClientServerMax = 10;
                ClientServerDistancePort = 100;
                ClientSelectedCharacter = client.BaseAddress + 0x549D34;
                ClientMultiClient = client.BaseAddress + 0x132347;

                ClientStatus = client.BaseAddress + 0x3C0CF8;

                PlayerGoX = client.BaseAddress + 0x583EA0;
                PlayerGoY = client.BaseAddress + 0x583E98;
                PlayerGoZ = client.BaseAddress + 0x54C004;

                ClientBattleListStart = client.BaseAddress + 0x54C008;
            }
            else if (client.Version == ClientVersion.Version986)
            {
                ClientRsa = client.BaseAddress + 0x32E0F8;
                ClientServerStart = client.BaseAddress + 0x3BF858;
                ClientServerStep = 112;
                ClientServerMax = 10;
                ClientServerDistancePort = 100;
                ClientSelectedCharacter = client.BaseAddress + 0x5501AC;
                ClientMultiClient = client.BaseAddress + 0x1341D0;

                ClientStatus = client.BaseAddress + 0x3C8FF8;

                PlayerGoX = client.BaseAddress + 0x58AEA0;
                PlayerGoY = client.BaseAddress + 0x58AE98;
                PlayerGoZ = client.BaseAddress + 0x553004;

                ClientBattleListStart = client.BaseAddress + 0x553008;
            }
            else if (client.Version == ClientVersion.Version1010)
            {
                ClientRsa = client.BaseAddress + 0x32E188;
                ClientServerStart = client.BaseAddress + 0x3BE840;
                ClientServerStep = 112;
                ClientServerMax = 10;
                ClientServerDistancePort = 100;
                ClientSelectedCharacter = client.BaseAddress + 0x54FE94;
                ClientMultiClient = client.BaseAddress + 0x134710;

                ClientStatus = client.BaseAddress + 0x3C7FE0;

                PlayerGoX = client.BaseAddress + 0x553030;
                PlayerGoY = client.BaseAddress + 0x553028;
                PlayerGoZ = client.BaseAddress + 0x553004;

                ClientBattleListStart = client.BaseAddress + 0x5A93D0;
            }
            else if (client.Version == ClientVersion.Version1011)
            {
                ClientRsa = client.BaseAddress + 0x32F288;
                ClientServerStart = client.BaseAddress + 0x40F31C;
                ClientServerEnd = client.BaseAddress + 0x40F320;
                ClientServerStep = 0x38;
                ClientServerDistanceHostname = 0x04;
                ClientServerDistanceIP = 0x20;
                ClientServerDistancePort = 0x30;
                ClientServerMax = 10;
                ClientSelectedCharacter = client.BaseAddress + 0x551AB4;
                ClientMultiClient = client.BaseAddress + 0x134F90;
                ClientProxyCheckFunctionPointer = client.BaseAddress + 0xE8660;

                ClientStatus = client.BaseAddress + 0x3C9BDC;

                PlayerGoX = client.BaseAddress + 0x554030;
                PlayerGoY = client.BaseAddress + 0x554028;
                PlayerGoZ = client.BaseAddress + 0x554004;

                ClientBattleListStart = client.BaseAddress + 0x5AA2E0;
            }
            else if (client.Version == ClientVersion.Version1012)
            {
                ClientRsa = client.BaseAddress + 0x330278;
                ClientServerStart = client.BaseAddress + 0x4102DC;
                ClientServerEnd = client.BaseAddress + 0x4102E0;
                ClientServerStep = 0x38;
                ClientServerDistanceHostname = 0x04;
                ClientServerDistanceIP = 0x20;
                ClientServerDistancePort = 0x30;
                ClientServerMax = 10;
                ClientSelectedCharacter = client.BaseAddress + 0x552A74;
                ClientMultiClient = client.BaseAddress + 0x136450;

                ClientStatus = client.BaseAddress + 0x3CAB9C;

                PlayerGoX = client.BaseAddress + 0x555030;
                PlayerGoY = client.BaseAddress + 0x555028;
                PlayerGoZ = client.BaseAddress + 0x555004;

                ClientBattleListStart = client.BaseAddress + 0x5AB2D0;
            }
            else if (client.Version == ClientVersion.Version1013)
            {
                ClientRsa = client.BaseAddress + 0x330278;
                ClientServerStart = client.BaseAddress + 0x4102DC;
                ClientServerEnd = client.BaseAddress + 0x4102E0;
                ClientServerStep = 0x38;
                ClientServerDistanceHostname = 0x04;
                ClientServerDistanceIP = 0x20;
                ClientServerDistancePort = 0x30;
                ClientServerMax = 10;
                ClientSelectedCharacter = client.BaseAddress + 0x552A74;
                ClientMultiClient = client.BaseAddress + 0x136450;

                ClientStatus = client.BaseAddress + 0x3CAB9C;

                PlayerGoX = client.BaseAddress + 0x555030;
                PlayerGoY = client.BaseAddress + 0x555028;
                PlayerGoZ = client.BaseAddress + 0x555004;

                ClientBattleListStart = client.BaseAddress + 0x5AB2D0;
            }
            else if (client.Version == ClientVersion.Version1020)
            {
                ClientRsa = client.BaseAddress + 0x331270;
                ClientServerStart = client.BaseAddress + 0x41130C;
                ClientServerEnd = client.BaseAddress + 0x411310;
                ClientServerStep = 0x38;
                ClientServerDistanceHostname = 0x04;
                ClientServerDistanceIP = 0x20;
                ClientServerDistancePort = 0x30;
                ClientServerMax = 10;
                ClientSelectedCharacter = client.BaseAddress + 0x553AA4;
                ClientMultiClient = client.BaseAddress + 0x136840;

                ClientStatus = client.BaseAddress + 0x3CBBBC;

                PlayerGoX = client.BaseAddress + 0x556030;
                PlayerGoY = client.BaseAddress + 0x556028;
                PlayerGoZ = client.BaseAddress + 0x556004;

                ClientBattleListStart = client.BaseAddress + 0x5AC238;
            }
            else if (client.Version == ClientVersion.Version1030)
            {
                ClientRsa = client.BaseAddress + 0x32FE78;
                ClientServerStart = client.BaseAddress + 0x41033C;
                ClientServerEnd = client.BaseAddress + 0x410340;
                ClientServerStep = 0x38;
                ClientServerDistanceHostname = 0x04;
                ClientServerDistanceIP = 0x20;
                ClientServerDistancePort = 0x30;
                ClientServerMax = 10;
                ClientSelectedCharacter = client.BaseAddress + 0x5530BC;
                ClientMultiClient = client.BaseAddress + 0x136770;

                ClientStatus = client.BaseAddress + 0x3CABAC;

                PlayerGoX = client.BaseAddress + 0x556030;
                PlayerGoY = client.BaseAddress + 0x556028;
                PlayerGoZ = client.BaseAddress + 0x556004;

                ClientBattleListStart = client.BaseAddress + 0x5AB890;
            }
            else if (client.Version == ClientVersion.Version1031)
            {
                ClientRsa = client.BaseAddress + 0x32FE60;
                ClientServerStart = client.BaseAddress + 0x41033C;
                ClientServerEnd = client.BaseAddress + 0x410340;
                ClientServerStep = 0x38;
                ClientServerDistanceHostname = 0x04;
                ClientServerDistanceIP = 0x20;
                ClientServerDistancePort = 0x30;
                ClientServerMax = 10;
                ClientSelectedCharacter = client.BaseAddress + 0x5530BC;
                ClientMultiClient = client.BaseAddress + 0x136770;

                ClientStatus = client.BaseAddress + 0x3CABAC;

                PlayerGoX = client.BaseAddress + 0x556030;
                PlayerGoY = client.BaseAddress + 0x556028;
                PlayerGoZ = client.BaseAddress + 0x556004;

                ClientBattleListStart = client.BaseAddress + 0x5AB298;
            }
            else if (client.Version == ClientVersion.Version1032)
            {
                ClientRsa = client.BaseAddress + 0x331300;
                ClientServerStart = client.BaseAddress + 0x4113CC;
                ClientServerEnd = client.BaseAddress + 0x4113D0;
                ClientServerStep = 0x38;
                ClientServerDistanceHostname = 0x04;
                ClientServerDistanceIP = 0x20;
                ClientServerDistancePort = 0x30;
                ClientServerMax = 10;
                ClientSelectedCharacter = client.BaseAddress + 0x55414C;
                ClientMultiClient = client.BaseAddress + 0x13C8B0;

                ClientStatus = client.BaseAddress + 0x3CBC3C;

                PlayerGoX = client.BaseAddress + 0x557030;
                PlayerGoY = client.BaseAddress + 0x557028;
                PlayerGoZ = client.BaseAddress + 0x557004;

                ClientBattleListStart = client.BaseAddress + 0x5AC558;
            }
            else if (client.Version == ClientVersion.Version1033)
            {
                ClientRsa = client.BaseAddress + 0x331300;
                ClientServerStart = client.BaseAddress + 0x4113CC;
                ClientServerEnd = client.BaseAddress + 0x4113D0;
                ClientServerStep = 0x38;
                ClientServerDistanceHostname = 0x04;
                ClientServerDistanceIP = 0x20;
                ClientServerDistancePort = 0x30;
                ClientServerMax = 10;
                ClientSelectedCharacter = client.BaseAddress + 0x55414C;
                ClientMultiClient = client.BaseAddress + 0x13C810;

                ClientStatus = client.BaseAddress + 0x3CBC3C;

                PlayerGoX = client.BaseAddress + 0x557030;
                PlayerGoY = client.BaseAddress + 0x557028;
                PlayerGoZ = client.BaseAddress + 0x557004;

                ClientBattleListStart = client.BaseAddress + 0x5AC558;
            }
            else if (client.Version == ClientVersion.Version1034)
            {
                ClientRsa = client.BaseAddress + 0x331320;
                ClientServerStart = client.BaseAddress + 0x4113CC;
                ClientServerEnd = client.BaseAddress + 0x4113D0;
                ClientServerStep = 0x38;
                ClientServerDistanceHostname = 0x04;
                ClientServerDistanceIP = 0x20;
                ClientServerDistancePort = 0x30;
                ClientServerMax = 10;
                ClientSelectedCharacter = client.BaseAddress + 0x55414C;
                ClientMultiClient = client.BaseAddress + 0x13C9C0;

                ClientStatus = client.BaseAddress + 0x3CBC3C;

                PlayerGoX = client.BaseAddress + 0x557030;
                PlayerGoY = client.BaseAddress + 0x557028;
                PlayerGoZ = client.BaseAddress + 0x557004;

                ClientBattleListStart = client.BaseAddress + 0x5AC558;
            }
            else if (client.Version == ClientVersion.Version1035)
            {
                ClientRsa = client.BaseAddress + 0x331320;
                ClientServerStart = client.BaseAddress + 0x4113CC;
                ClientServerEnd = client.BaseAddress + 0x4113D0;
                ClientServerStep = 0x38;
                ClientServerDistanceHostname = 0x04;
                ClientServerDistanceIP = 0x20;
                ClientServerDistancePort = 0x30;
                ClientServerMax = 10;
                ClientSelectedCharacter = client.BaseAddress + 0x55414C;
                ClientMultiClient = client.BaseAddress + 0x13CA20;

                ClientStatus = client.BaseAddress + 0x3CBC3C;

                PlayerGoX = client.BaseAddress + 0x557030;
                PlayerGoY = client.BaseAddress + 0x557028;
                PlayerGoZ = client.BaseAddress + 0x557004;

                ClientBattleListStart = client.BaseAddress + 0x5AC558;
            }
            else if (client.Version == ClientVersion.Version1041)
            {
                ClientRsa = client.BaseAddress + 0x33A450;
                ClientServerStart = client.BaseAddress + 0x41C568;
                ClientServerEnd = client.BaseAddress + 0x41C56C;
                ClientServerStep = 0x38;
                ClientServerDistanceHostname = 0x04;
                ClientServerDistanceIP = 0x20;
                ClientServerDistancePort = 0x30;
                ClientServerMax = 10;
                ClientSelectedCharacter = client.BaseAddress + 0x55F4CC;
                ClientMultiClient = client.BaseAddress + 0x1413D0;

                ClientStatus = client.BaseAddress + 0x3D6D80;

                PlayerGoX = client.BaseAddress + 0x562030;
                PlayerGoY = client.BaseAddress + 0x562028;
                PlayerGoZ = client.BaseAddress + 0x562004;

                ClientBattleListStart = client.BaseAddress + 0x5B8188;
            }
            else if (client.Version == ClientVersion.Version1050)
            {
                ClientRsa = client.BaseAddress + 0x36F7F0;
                ClientServerStart = client.BaseAddress + 0x461738;
                ClientServerEnd = client.BaseAddress + 0x46173C;
                ClientServerStep = 0x30;
                ClientServerDistanceHostname = 0x04;
                ClientServerDistanceIP = 0x1C;
                ClientServerDistancePort = 0x28;
                ClientServerMax = 10;
                ClientSelectedCharacter = client.BaseAddress + 0x5A43DC;
                ClientMultiClient = client.BaseAddress + 0x146167;

                ClientStatus = client.BaseAddress + 0x31CA38;

                PlayerGoX = client.BaseAddress + 0x5A7030;
                PlayerGoY = client.BaseAddress + 0x5A7028;
                PlayerGoZ = client.BaseAddress + 0x5A7004;

                ClientBattleListStart = client.BaseAddress + 0x5F8B00;
            }
            else if (client.Version == ClientVersion.Version1056)
            {
                ClientRsa = client.BaseAddress + 0x37E900;
                ClientServerStart = client.BaseAddress + 0x474DEC;
                ClientServerEnd = client.BaseAddress + 0x474DF0;
                ClientServerStep = 0x30;
                ClientServerDistanceHostname = 0x04;
                ClientServerDistanceIP = 0x1C;
                ClientServerDistancePort = 0x28;
                ClientServerMax = 10;
                ClientSelectedCharacter = client.BaseAddress + 0x5B7B44;
                ClientMultiClient = client.BaseAddress + 0x14E977;

                ClientStatus = client.BaseAddress + 0x31CA38;//?

                PlayerGoX = client.BaseAddress + 0x5BA030;
                PlayerGoY = client.BaseAddress + 0x5BA028;
                PlayerGoZ = client.BaseAddress + 0x5BA004;

                ClientBattleListStart = client.BaseAddress + 0x60E5F0;
            }
            else if (client.Version == ClientVersion.Version1057)
            {
                ClientRsa = client.BaseAddress + 0x382930;
                ClientServerStart = client.BaseAddress + 0x483E54;
                ClientServerEnd = client.BaseAddress + 0x483E58;
                ClientServerStep = 0x30;
                ClientServerDistanceHostname = 0x04;
                ClientServerDistanceIP = 0x1C;
                ClientServerDistancePort = 0x28;
                ClientServerMax = 10;
                ClientSelectedCharacter = client.BaseAddress + 0x5C6BBC;
                ClientMultiClient = client.BaseAddress + 0x1524E7;

                ClientStatus = client.BaseAddress + 0x31CA38;//?

                PlayerGoX = client.BaseAddress + 0x5C9030;
                PlayerGoY = client.BaseAddress + 0x5C9028;
                PlayerGoZ = client.BaseAddress + 0x5C9004;

                ClientBattleListStart = client.BaseAddress + 0x61D730;
            }
            else if (client.Version == ClientVersion.Version1059)
            {
                ClientRsa = client.BaseAddress + 0x382930;
                ClientServerStart = client.BaseAddress + 0x483E54;
                ClientServerEnd = client.BaseAddress + 0x483E58;
                ClientServerStep = 0x30;
                ClientServerDistanceHostname = 0x04;
                ClientServerDistanceIP = 0x1C;
                ClientServerDistancePort = 0x28;
                ClientServerMax = 10;
                ClientSelectedCharacter = client.BaseAddress + 0x5C6BBC;
                ClientMultiClient = client.BaseAddress + 0x1524E7;

                ClientStatus = client.BaseAddress + 0x31CA38;//?

                PlayerGoX = client.BaseAddress + 0x5C9030;
                PlayerGoY = client.BaseAddress + 0x5C9028;
                PlayerGoZ = client.BaseAddress + 0x5C9004;

                ClientBattleListStart = client.BaseAddress + 0x61D730;
            }
            else if (client.Version == ClientVersion.Version1071)
            {
                ClientRsa = client.BaseAddress + 0x3879A0;
                ClientServerStart = client.BaseAddress + 0x48C264;
                ClientServerEnd = client.BaseAddress + 0x48C268;
                ClientServerStep = 0x30;
                ClientServerDistanceHostname = 0x04;
                ClientServerDistanceIP = 0x1C;
                ClientServerDistancePort = 0x28;
                ClientServerMax = 10;
                ClientSelectedCharacter = client.BaseAddress + 0x5CF07C;
                ClientMultiClient = client.BaseAddress + 0x156557;

                ClientStatus = client.BaseAddress + 0x31CA38;//?

                PlayerGoX = client.BaseAddress + 0x5D2030;
                PlayerGoY = client.BaseAddress + 0x5D2028;
                PlayerGoZ = client.BaseAddress + 0x5D2004;

                ClientBattleListStart = client.BaseAddress + 0x6296C0;
            }
            else if (client.Version == ClientVersion.Version1073)
            {
                ClientRsa = client.BaseAddress + 0x3899A0;
                ClientServerStart = client.BaseAddress + 0x48F5A8;
                ClientServerEnd = client.BaseAddress + 0x48F5AC;
                ClientServerStep = 0x30;
                ClientServerDistanceHostname = 0x04;
                ClientServerDistanceIP = 0x1C;
                ClientServerDistancePort = 0x28;
                ClientServerMax = 10;
                ClientSelectedCharacter = client.BaseAddress + 0x5D23D4;
                ClientMultiClient = client.BaseAddress + 0x157FE7;

                ClientStatus = client.BaseAddress + 0x31CA38;//?

                PlayerGoX = client.BaseAddress + 0x5D5030;
                PlayerGoY = client.BaseAddress + 0x5D5028;
                PlayerGoZ = client.BaseAddress + 0x5D5004;

                ClientBattleListStart = client.BaseAddress + 0x62CA30;
            }
            else if (client.Version == ClientVersion.Version1074)
            {
                ClientRsa = client.BaseAddress + 0x3889A0;
                ClientServerStart = client.BaseAddress + 0x48E5B8;
                ClientServerEnd = client.BaseAddress + 0x48E5BC;
                ClientServerStep = 0x30;
                ClientServerDistanceHostname = 0x04;
                ClientServerDistanceIP = 0x1C;
                ClientServerDistancePort = 0x28;
                ClientServerMax = 10;
                ClientSelectedCharacter = client.BaseAddress + 0x5D13FC;
                ClientMultiClient = client.BaseAddress + 0x157D27;

                ClientStatus = client.BaseAddress + 0x31CA38;//?

                PlayerGoX = client.BaseAddress + 0x5D4030;
                PlayerGoY = client.BaseAddress + 0x5D4028;
                PlayerGoZ = client.BaseAddress + 0x5D4004;

                ClientBattleListStart = client.BaseAddress + 0x62B7D0;
            }
            else if (client.Version == ClientVersion.Version1075)
            {
                ClientRsa = client.BaseAddress + 0x3889A0;
                ClientServerStart = client.BaseAddress + 0x48E5B8;
                ClientServerEnd = client.BaseAddress + 0x48E5BC;
                ClientServerStep = 0x30;
                ClientServerDistanceHostname = 0x04;
                ClientServerDistanceIP = 0x1C;
                ClientServerDistancePort = 0x28;
                ClientServerMax = 10;
                ClientSelectedCharacter = client.BaseAddress + 0x5D13FC;
                ClientMultiClient = client.BaseAddress + 0x157C97;

                ClientStatus = client.BaseAddress + 0x31CA38;//?

                PlayerGoX = client.BaseAddress + 0x5D4030;
                PlayerGoY = client.BaseAddress + 0x5D4028;
                PlayerGoZ = client.BaseAddress + 0x5D4004;

                ClientBattleListStart = client.BaseAddress + 0x62B7D0;
            }
            else if (client.Version == ClientVersion.Version1076)
            {
                ClientRsa = client.BaseAddress + 0x3899A0;
                ClientServerStart = client.BaseAddress + 0x48F5E0;
                ClientServerEnd = client.BaseAddress + 0x48F5E4;
                ClientServerStep = 0x30;
                ClientServerDistanceHostname = 0x04;
                ClientServerDistanceIP = 0x1C;
                ClientServerDistancePort = 0x28;
                ClientServerMax = 10;
                ClientSelectedCharacter = client.BaseAddress + 0x5D2424;
                ClientMultiClient = client.BaseAddress + 0x158587;

                ClientStatus = client.BaseAddress + 0x31CA38;//?

                PlayerGoX = client.BaseAddress + 0x5D5030;
                PlayerGoY = client.BaseAddress + 0x5D5028;
                PlayerGoZ = client.BaseAddress + 0x5D5004;

                ClientBattleListStart = client.BaseAddress + 0x62C0D0;
            }
            else
            {
                throw new Exception("The client version " + client.Version + " is not supported.");
            }
        }
    }
}
