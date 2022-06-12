---
id: FriendsGraph
title: Friends Graph
permalink: /docs/manual/DemoApps/FriendsGraph.html
---

##### <a href="https://github.com/Microsoft/GraphEngine/tree/master/samples/Friends" target="_blank">Source Code on GitHub</a>

This example is to illustrate how to model and store data in GE.  In this "hello
world" application, we model and store a small social graph of the six main
characters in the famous situation comedy *Friends*.

We model two types of entities in this example. One is the Characters in the TV
series, the other is their Performers. _Friends_ has six main characters as
shown in the Characters table. There are many relations between these entities.
In this example, we focus on the following three relationships: the _marriage_
relationship between two characters, the _portray_ relationship between a
performer and a Character, and the _friendship_ relationship between two
characters. The properties of each entity are shown below.

{% include FriendsGraph-tables.html %}

We first create a TSL script to model the data shown in the table. The
script defines the schema of the data.

```C#
cell struct Character
{
	String Name;
	byte Gender;
	bool Married;
	long Spouse;
	long Performer;
}

cell struct Performer
{
	String Name;
	int Age;
	List<long> Characters;
}
```

In Visual Studio, we can now _New_ a _Graph Engine Project_
called _FriendsCell_.  We copy and paste the tsl script shown above
into the default TSL file.

Next, we _New_ a _Graph Engine Application Project_ called _Friends_.
Open the generated Program.cs of _Friends_ project, add the following
namespaces to the preamble:

```C#
using Trinity;
using Trinity.Extension;
```

A client-side Trinity program has two running modes. One is
_Embedded_ mode, and the other is _Client_ mode.  Let us go with the
simpler _embedded_ mode first. We can instruct Trinity to
run in _embedded_ mode by putting the following line at the beginning
of the program.

```C#
TrinityConfig.CurrentRunningMode = RunningMode.Embedded;
```

Now we can get into the real business. There are six characters and
six corresponding performers. We first create 12 entity cells for
them:

```C#
// Characters
Character Rachel = new Character(Name: "Rachel Green", Gender: 0, Married: true);
Character Monica = new Character(Name: "Monica Geller", Gender: 0, Married: true);
Character Phoebe = new Character(Name: "Phoebe Buffay", Gender: 0, Married: true);
Character Joey = new Character(Name: "Joey Tribbiani", Gender: 1, Married: false);
Character Chandler = new Character(Name: "Chandler Bing", Gender: 1, Married: true);
Character Ross = new Character(Name: "Ross Geller", Gender: 1, Married: true);

// Cast
Performer Jennifer = new Performer(Name: "Jennifer Aniston", Age: 43,
Characters: new List<long>());
Performer Courteney = new Performer(Name: "Courteney Cox", Age: 48,
Characters: new List<long>());
Performer Lisa = new Performer(Name: "Lisa Kudrow", Age: 49,
Characters: new List<long>());
Performer Matt = new Performer(Name: "Matt Le Blanc", Age: 45,
Characters: new List<long>());
Performer Matthew = new Performer(Name: "Matthew Perry", Age: 43,
Characters: new List<long>());
Performer David = new Performer(Name: "David Schwimmer", Age: 45,
Characters: new List<long>());
```

Now we define a portrayal relationship to illustrate how we represent
directed relationships. A _Portrayal_ relationship is the relationship
between a performer and a character. It is about _who performs a
character_. For example, _Jennifer_ _portrays_ the character of
_Rachel_.

```C#
Rachel.Performer = Jennifer.CellID;
Jennifer.Characters.Add(Rachel.CellID);
```

Then, we define an undirected marriage relationship to illustrate how
undirected relationships are represented. For example, _Monica_ and
_Chandler_ are _spouse_ of each other in the show.

```C#
Monica.Spouse = Chandler.CellID;
Chandler.Spouse = Monica.CellID;
```

We can easily model multilateral relationships as well. To illustrate
this, let us consider the _friends_ relationship between these six
characters. All these six characters are _friends_ with each
other. That means we need to create 15 _friends_ relationship edges
between each pair of them. How to simplify this?

In Graph Engine, we can create a _hyperedge_ cell called
_Friendship_ and connect all these six characters using this cell. To
do this, we extend the _MyCell.tsl_ script a bit. We add the following
three lines to this file to create a _Friendship_ cell.

```C#
cell struct Friendship
{
	List<long> friends;
}
```

After adding the _Friendship_ cell definition, recompile the
_FriendsCell_ project. Now we can make the six guys _friends_ in our
program.

```C#
Friendship friend_ship = new Friendship(new List<long>());
friend_ship.friends.Add(Rachel.CellID);
friend_ship.friends.Add(Monica.CellID);
friend_ship.friends.Add(Phoebe.CellID);
friend_ship.friends.Add(Joey.CellID);
friend_ship.friends.Add(Chandler.CellID);
friend_ship.friends.Add(Ross.CellID);
```

So far so good.  We have 12 entity cells and we define three
relationships between these entities. But wait, we are not done
yet. All these cells now are no more than 12 .Net objects on .Net
runtime heap. It is neither in Trinity's managed main memory
storage, nor persisted on disk files.

Let us see how to make data persistent in Trinity. _Cell_ is the basic
data unit in Trinity. A cell may exist in three forms.

* As a .Net object on .Net runtime heap.
* As a blob of bytes in Trinity's memory storage.
* As a blob of bytes in disk files.

For now, the cells we created are on the .Net runtime heap. To
leverage the true power of Trinity, we need to save these
cells to Trinity's main memory storage. The reasons why we
need to do this was discussed in
[Accessors](/docs/manual/TSL/accessor.html). With cells stored in the
Trinity memory storage, we have thread-safe cell
manipulation guarantee without losing the convenience of
object-oriented cell accessing interfaces.

Runtime objects can be easily converted into cells resident in
Trinity memory storage.  We can save a _Performer_ cell by
calling _Global.LocalStorage.SavePerformer(performer)_ and save a
Character cell by calling
_Global.LocalStorage.SaveCharacter(character)_ as follows.

```C#
Global.LocalStorage.SavePerformer(Jennifer);
Global.LocalStorage.SaveCharacter(Rachel);
```

Once the data is in Trinity's LocalStorage, we can persist
the data to the disk by simply calling
_Global.LocalStorage.SaveStorage()_. Below is a complete List of our
first _hello world_ Trinity program.

```C#
using System;
using System.Collections.Generic;
using System.Text;

using Trinity;
using Trinity.Data;
using Trinity.Storage;

namespace Friends
{
    class Friends
    {
        public unsafe static void Main(string[] args)
        {
            TrinityConfig.CurrentRunningMode = RunningMode.Embedded;

            // Characters
            Character Rachel = new Character(Name: "Rachel Green", Gender: 0,
            Married: true);
            Character Monica = new Character(Name: "Monica Geller", Gender: 0,
            Married: true);
            Character Phoebe = new Character(Name: "Phoebe Buffay", Gender: 0,
            Married: true);
            Character Joey = new Character(Name: "Joey Tribbiani", Gender: 1,
            Married: false);
            Character Chandler = new Character(Name: "Chandler Bing", Gender: 1,
            Married: true);
            Character Ross = new Character(Name: "Ross Geller", Gender: 1,
            Married: true);

            // Performers
            Performer Jennifer = new Performer(Name: "Jennifer Aniston", Age: 43,
            Characters: new List<long>());
            Performer Courteney = new Performer(Name: "Courteney Cox", Age: 48,
            Characters: new List<long>());
            Performer Lisa = new Performer(Name: "Lisa Kudrow", Age: 49,
            Characters: new List<long>());
            Performer Matt = new Performer(Name: "Matt Le Blanc", Age: 45,
            Characters: new List<long>());
            Performer Matthew = new Performer(Name: "Matthew Perry", Age: 43,
            Characters: new List<long>());
            Performer David = new Performer(Name: "David Schwimmer", Age: 45,
            Characters: new List<long>());

            // Portrayal Relationship
            Rachel.Performer = Jennifer.CellID;
            Jennifer.Characters.Add(Rachel.CellID);

            Monica.Performer = Courteney.CellID;
            Courteney.Characters.Add(Monica.CellID);

            Phoebe.Performer = Lisa.CellID;
            Lisa.Characters.Add(Phoebe.CellID);

            Joey.Performer = Matt.CellID;
            Matt.Characters.Add(Joey.CellID);

            Chandler.Performer = Matthew.CellID;
            Matthew.Characters.Add(Chandler.CellID);

            Ross.Performer = David.CellID;
            David.Characters.Add(Ross.CellID);

            // Marriage relationship
            Monica.Spouse = Chandler.CellID;
            Chandler.Spouse = Monica.CellID;

            Rachel.Spouse = Ross.CellID;
            Ross.Spouse = Rachel.CellID;

            // Friendship
            Friendship friend_ship = new Friendship(new List<long>());
            friend_ship.friends.Add(Rachel.CellID);
            friend_ship.friends.Add(Monica.CellID);
            friend_ship.friends.Add(Phoebe.CellID);
            friend_ship.friends.Add(Joey.CellID);
            friend_ship.friends.Add(Chandler.CellID);
            friend_ship.friends.Add(Ross.CellID);

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

            // Dump memory storage to disk for persistence
            Global.LocalStorage.SaveStorage();

            long spouse_id = -1;

            using (var cm = Global.LocalStorage.UseCharacter(Monica.CellID))
            {
                if (cm.Married)
                    spouse_id = cm.Spouse;
            }

            using (var cm = Global.LocalStorage.UseCharacter(spouse_id))
            {
                Console.WriteLine(cm.Name);
            }
        }
    }
}
```

The lines after _Global.LocalStorage.SaveStorage()_ are used to
verify the data we have stored. We will explain them in the later
chapters.
