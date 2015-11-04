![ShipSections Logo](https://raw.githubusercontent.com/jkoritzinsky/ShipSections/master/ShipSections%20Banner.png)

Welcome to ShipSections
===

What is ShipSections?
---

ShipSections lets you divide your ships into different sections like "Lifter", "Interplanetary", "Lander", "Explodey Piece", etc.  Through ShipSections, other mods can analyze just the parts within that section. Note: This is currently a VAB/SPH only experience.  ShipSections does not currently have full support for craft in flight.  It works when nothing blows up and nothing is docked, but I don't make any promises.

How do I use ShipSections? (Player)
---

The ShipSections window is on the App Launcher.  Click it, and you'll see a window with all of the current sections and a button to highlight each of them.
To create a section, right click on any decoupler or non-shielded docking port and click "Create Section".  That will make this part and all of its children within the current section parts of a new section.  You can rename the section using the window mentioned before.

How do I use ShipSections? (Modder)
---
1. Reference the ShipSections.dll.
2. Derive your class from `SectionData<Your Class>`
3. Implement the Merge method. This method takes in another instance of your data class.  Your job is to merge the data from that instance into your instance.  This happens upon initial loading (merge current data into defaults), and whenever sections are merged.
4. Optional: Create a config file with default values.

Creating a config file with default values
---
Here is an example config:
```
SECTIONDATADEF
{
    name = MySectionData
    isPayload = 1
}
```


```cs
public class MySectionData : SectionData<MySectionData>
{
    [Persistent]
    public int isPayload = 0;
    
    protected override void Merge(MySectionData data)
    {
        isPayload = data.isPayload;
    }
}
```

Getting your section data
---
The following method can be used to get data:
```
T API.GetSectionDataForMod<T>(string sectionName)
```
Pass in the name of a section and the type of section data that you want, and this method will return it to you for you to use. Generally, you should only have to use the API class and the `SectionData<T>` class.

Reporting Bugs
===

If you find a bug or want a new feature, contact me either on the KSP Forums, on [GitHub](https://github.com/jkoritzinsky/ShipSections/issues), or on the [KSP](reddit.com/r/KerbalSpaceProgram) or [KSP Mod Dev](reddit.com/r/KSPModDevelopment) subreddits, or by private message on any of these platforms.


Mods that Use ShipSections
===
Extensive Engineer Report v0.5+