using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

namespace RajceInternal.Features.Misc
{
    internal class NickChanger : FeatureBase
    {
        // Start
        private string[] Prefixes = new string[]
        {
            "The",
            "Da",
            "Fat",
            "Average",
            "Biggest",
            "Fattes",
            "Chlapikest",
            "Coded",
            "Swagger",
            "Project",
            "Mr",
            "Ms",
            "Evil",
            "Angry",
            "Jewish",
            "Blackish",
            "Daily",
            "Chocolate",
            "Hax",
            "Simple",
            "Top",
            "BMW",
            "Don",
            "AWP",
            "Panic",
            "Lidl",
            "KFC",
            "Mc",
            "RP",
            "Piko"
        };

        // Middle
        private string[] Names = new string[]
        {
            "Drawing",
            "Cold",
            "Fire",
            "Games",
            "Boost",
            "Kubinek",
            "Chlapik",
            "Tomato",
            "Player",
            "Trash",
            "Marek",
            "Taras",
            "Petr",
            "Candice",
            "Evzen",
            "Cigan",
            "Fotr",
            "Siberia",
            "Venix",
            "Kreten",
            "Scammer",
            "Insider",
            "Fallen",
            "Scientist",
            "Donut",
            "Donat",
            "Shooter",
            "Alex",
            "Bot",
            "Parte",
            "Pepik",
            "Ninja",
            "Solider",
            "Jerry",
            "Frodo",
            "TripleS",
            "Gray",
            "Grey",
            "Boss",
            "Man",
            "Pixel",
            "Ship",
            "Chat",
            "Sunny",
            "Peanut",
            "Kasicka",
            "Mondar",
            "Jezevec",
            "Barber",
            "Chucky",
            "Beast",
            "Rumburak",
            "Aladin",
            "Shangai",
            "Lada",
            "Ramon",
            "Kenny",
            "Bastard",
            "Vodnar",
            "Markus",
            "Rodic",
            "Arrow",
            "VED",
            "Konor",
            "Bunda",
            "Clovek",
            "Ant",
            "Saliery",
            "Mrazik",
            "Lakatos",
            "Mikula",
            "Brikula",
            "Krypl",
            "Burger",
            "Anatomy",
            "Doll",
            "Topka",
            "Omar",
            "Letadlo",
            "Dvojce",
            "Boom",
            "Gordon",
            "Ramsey",
            "Drainul",
            "Rasto",
            "Ocean",
            "Nobody",
            "Kubco",
            "CkGang",
            "Crazysek",
            "Carpet",
            "Mascarpone",
            "Kaves",
            "Bejros",
            "Muzicka",
            "Chuj",
            "Tygr",
            "Semtex",
            "Monster",
            "Lekarnik",
            "Twisster",
            "Fister",
            "Spy",
            "Petarda",
            "Glock",
            "Pirat",
        };

        // End
        private string[] Sufixes = new string[]
        {
            "LP",
            "CZ",
            "XxX",
            "Live",
            "Gaming",
            "CS",
            "Wolf",
            "Lore",
            "Clips",
            "God",
            "AFK",
            "King",
            "Princ",
            "Fried",
            "CZSK",
            "CZLP",
            "TV",
            "Extra",
            "Bussines",
            "Channel",
            "Two",
            "Memes",
            "Fancy",
            "Vlog",
            "Mafian",
            "RU",
            "EU",
            "HUN",
            "PL",
            "RO",
            "UA",
            "SK",
            "Gansta",
            "Zelezity"
        };

        private System.Random _rnd = new System.Random();
        private bool IsRunning = false;

        public override string Name { get; protected set; } = "Nick Changer";
        public override string Description { get; protected set; } = "Changes your nick once in a while";
        public override bool IsKeyBindable { get; protected set; } = false;
        public override KeyCode BindedKey { get; set; }

        private IEnumerator<object> ChangeNick()
        {
            IsRunning = true;

            while (IsActive && m_bIsConnected)
            {
                NicknameSync sync = PlayerManager.localPlayer.GetComponent<NicknameSync>();
                if (sync == null)
                {
                    Console.WriteLine("Failed to get nicksync");
                    yield return new WaitForEndOfFrame();
                }

                string name = Names[_rnd.Next(0, Names.Length)];
                sync.CallCmdSetNick(name);

                Console.WriteLine("Changed nick");
                yield return new WaitForSeconds(10f);
            }

            IsRunning = false;
            yield return null;
        }

        public override void OnEnable()
        {
            if (!IsRunning)
                Main._Coroutine.StartCoroutine(ChangeNick());
        }

        public override void OnConnected()
        {
            if (!IsRunning)
                Main._Coroutine.StartCoroutine(ChangeNick());
        }
    }
}
