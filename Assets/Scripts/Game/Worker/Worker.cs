/*
 * A worker which contributes to the development of products.
 * "Worker" here is used abstractly - it can mean an employee, or a location, or a planet.
 * i.e. a worker is some _productive_ entity.
 * This is the immutable template of a Worker, which is instantiated as AWorker
 */

using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

[System.Serializable]
public class Worker : SharedResource<Worker> {
    public static List<AWorker> LoadAll() {
        // Load workers as _copies_ so any changes don't get saved to the actual resources.
        // This does not load special workers, since they are only added to available workers through effects.
        return Resources.LoadAll<Worker>("Workers/Bulk").ToList().Select(w => {
                return new AWorker(w);
        }).ToList();
    }

    public static new Worker Load(string name) {
        Worker w = Resources.Load("Workers/Bulk/" + name) as Worker;
        if (w == null) {
            w = Resources.Load("Founders/" + name) as Worker;
        }
        if (w == null) {
            w = Resources.Load("Founders/Cofounders/" + name) as Worker;
        }
        return w;
    }

    // The worker's avatar in the game world.
    [HideInInspector]
    public GameObject avatar;

    // Each worker has their own style!
    public Material material;

    // Later on, there are robotic workers.
    public bool robot = false;

    public string description;
    public string title;
    public List<string> bio;
    public List<Preference> personalInfo;

    public float happiness;
    public float productivity;
    public float charisma;
    public float creativity;
    public float cleverness;
    public float baseMinSalary;

    public List<string> personalInfos {
        get {
            List<string> pis = new List<string>();
            foreach (Preference p in personalInfo) {
                pis.Add(p.ToString());
            }
            return pis;
        }
    }

    // WORKER BIO GENERATION ---------------------------

    private static Dictionary<string, Dictionary<string, string[]>> bioMap = new Dictionary<string, Dictionary<string, string[]>> {
        {
            "Creativity", new Dictionary<string, string[]> {
                { "low", new string[] {"is not very imaginative", "is often completely uninspiring"} },
                { "mid", new string[] {"is artistic", "has an eye for trends", "can be quite original"} },
                { "high", new string[] {"is a genius", "is a visionary", "is extremely talented"} }
            }
        },
        {
            "Cleverness", new Dictionary<string, string[]> {
                { "low", new string[] {"is quite dim", "isn't the sharpest tool in the shed", "isn't the brightest bulb of the bunch"} },
                { "mid", new string[] {"is an adept problem solver", "is technically gifted", "solves problems well", "is inventive", "can wear many hats"} },
                { "high", new string[] {"has a beautiful mind", "is absolutely brilliant", "is always on the cutting edge"} }
            }
        },
        {
            "Charisma", new Dictionary<string, string[]> {
                { "low", new string[] {"is alienating", "is hard to be around", "has a weird smell", "is very awkward", "creeps me out"} },
                { "mid", new string[] {"is fun to be around", "can be charming", "is easy to talk to", "is friendly"} },
                { "high", new string[] {"has a magnetic personality", "is hypnotic", "is a natural-born leader", "can be very persuasive", "is capable of rallying people to anything"} }
            }
        },
        {
            "Productivity", new Dictionary<string, string[]> {
                { "low", new string[] {"is kind of lazy", "needs a lot of motivation", "slacks off from time to time"} },
                { "mid", new string[] {"is a diligent worker", "is dedicated", "works hard", "is efficient with time"} },
                { "high", new string[] {"is really 'heads-down'", "multitasks very effectively", "gets a lot done in a short time"} }
            }
        },
        {
            "Happiness", new Dictionary<string, string[]> {
                { "low", new string[] {"is a real bummer", "brings everyone down", "reminds me of Eeyore", "can be very tempermental"} },
                { "mid", new string[] {"brightens up the office", "cheers everyone up", "is very nice"} },
                { "high", new string[] {"would be a great cultural fit", "is a real pleasure to work with", "has an infectious energy"} }
            }
        }
    };

    // There are certain preferences for which
    // employees are willing to take less salary
    // if mentioned in conversation
    public enum Preference {
        HEALTHCARE,
        VACATION,
        CULTURE,
        LOANS, // student loans
        DEBT, // credit card debt
        DENTAL,
        VISION,
        RETIREMENT,
        TUITION,
        PARENTAL,
        FITNESS,
        DISCOUNTS,
    }

    public static List<Preference> BuildPreferences(Worker w) {
        List<Preference> prefs = new List<Preference>();
        foreach (Preference p in Enum.GetValues(typeof(Preference)).Cast<Preference>()) {
            if (UnityEngine.Random.value < 0.25) {
                prefs.Add(p);
            }
        }
        return prefs;
    }

    public static List<string> BuildBio(Worker w) {
        // Randomize order of stats.
        string[] stats = new string[] {"Creativity", "Cleverness", "Charisma", "Productivity", "Happiness"};
        stats = stats.OrderBy(x => UnityEngine.Random.value).ToArray();

        List<string> bio = new List<string>();

        foreach (string stat in stats) {
            float val = w.StatByName(stat);
            string level = SkillLevel(val);

            string[] descs = bioMap[stat][level];
            int idx = UnityEngine.Random.Range(0,(descs.Length - 1));
            bio.Add(descs[idx]);
        }
        return bio;
    }

    private static string SkillLevel(float val) {
        if (val <= 4)
            return "low";
        if (val <= 8)
            return "mid";
        else
            return "high";
    }

    public float StatByName(string name) {
        switch (name) {
            case "Happiness":
                return happiness;
            case "Productivity":
                return productivity;
            case "Charisma":
                return charisma;
            case "Creativity":
                return creativity;
            case "Cleverness":
                return cleverness;
            default:
                return 0f;
        }
    }
}


