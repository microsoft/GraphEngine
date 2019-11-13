// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Trinity;
using Trinity.Storage;

namespace Friends
{
    class Friends
    {
        public static unsafe void Main(string[] args)
        {
            TrinityConfig.CurrentRunningMode = RunningMode.Embedded;
            // Characters
            Character Rachel = new Character(Name: "Rachel Green", Gender: 0, Married: true);
            Character Monica = new Character(Name: "Monica Geller", Gender: 0, Married: true);
            Character Phoebe = new Character(Name: "Phoebe Buffay", Gender: 0, Married: true);
            Character Joey = new Character(Name: "Joey Tribbiani", Gender: 1, Married: false);
            Character Chandler = new Character(Name: "Chandler Bing", Gender: 1, Married: true);
            Character Ross = new Character(Name: "Ross Geller", Gender: 1, Married: true);

            // Performers
            Performer Jennifer = new Performer(Name: "Jennifer Aniston", Age: 43, Characters: new List<long>());
            Performer Courteney = new Performer(Name: "Courteney Cox", Age: 48, Characters: new List<long>());
            Performer Lisa = new Performer(Name: "Lisa Kudrow", Age: 49, Characters: new List<long>());
            Performer Matt = new Performer(Name: "Matt Le Blanc", Age: 45, Characters: new List<long>());
            Performer Matthew = new Performer(Name: "Matthew Perry", Age: 43, Characters: new List<long>());
            Performer David = new Performer(Name: "David Schwimmer", Age: 45, Characters: new List<long>());

            // Portrayal Relationship
            Rachel.Performer = Jennifer.CellId;
            Jennifer.Characters.Add(Rachel.CellId);

            Monica.Performer = Courteney.CellId;
            Courteney.Characters.Add(Monica.CellId);

            Phoebe.Performer = Lisa.CellId;
            Lisa.Characters.Add(Phoebe.CellId);

            Joey.Performer = Matt.CellId;
            Matt.Characters.Add(Joey.CellId);

            Chandler.Performer = Matthew.CellId;
            Matthew.Characters.Add(Chandler.CellId);

            Ross.Performer = David.CellId;
            David.Characters.Add(Ross.CellId);

            // Marriage relationship
            Monica.Spouse = Chandler.CellId;
            Chandler.Spouse = Monica.CellId;

            Rachel.Spouse = Ross.CellId;
            Ross.Spouse = Rachel.CellId;

            // Friendship
            Friendship friend_ship = new Friendship(new List<long>());
            friend_ship.friends.Add(Rachel.CellId);
            friend_ship.friends.Add(Monica.CellId);
            friend_ship.friends.Add(Phoebe.CellId);
            friend_ship.friends.Add(Joey.CellId);
            friend_ship.friends.Add(Chandler.CellId);
            friend_ship.friends.Add(Ross.CellId);
            Global.LocalStorage.SaveFriendship(friend_ship);

            // Save Runtime cells to Trinity memory storage
            Global.LocalStorage.SavePerformer(Jennifer);
            Global.LocalStorage.SavePerformer(Courteney);
            Global.LocalStorage.SavePerformer(Lisa);
            Global.LocalStorage.SavePerformer(Matt);
            Global.LocalStorage.SavePerformer(Matthew);
            Global.LocalStorage.SavePerformer(David);

            Global.LocalStorage.SaveCharacter(Rachel);
            Global.LocalStorage.SaveCharacter(Monica);
            Global.LocalStorage.SaveCharacter(Phoebe);
            Global.LocalStorage.SaveCharacter(Joey);
            Global.LocalStorage.SaveCharacter(Chandler);
            Global.LocalStorage.SaveCharacter(Ross);

            Console.WriteLine("The character list: ");

            foreach (var character in Global.LocalStorage.Character_Accessor_Selector())
            {
                Console.WriteLine(character.Name);
            }

            Console.WriteLine();
            Console.WriteLine("The performer list: ");
            foreach (var performer in Global.LocalStorage.Performer_Accessor_Selector())
            {
                Console.WriteLine(performer.Name);
            }

            Console.WriteLine();

            long spouse_id = -1;
            using (var cm = Global.LocalStorage.UseCharacter(Monica.CellId))
            {
                if (cm.Married)
                    spouse_id = cm.Spouse;
            }

            Console.Write("The spouse of Monica is: ");

            using (var cm = Global.LocalStorage.UseCharacter(spouse_id))
            {
                Console.WriteLine(cm.Name);
            }

            Console.WriteLine("Press any key to exit ...");
            Console.ReadKey();
        }
    }
}

