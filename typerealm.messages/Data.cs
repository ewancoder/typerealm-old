using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TypeRealm.Messages;

namespace TypeRealm.Data
{
    public struct Damage
    {
        public Damage(int min, int max)
        {
            Min = min;
            Max = max;
        }

        public int Min { get; }
        public int Max { get; }

        public override string ToString()
        {
            if (Min == 0 && Max == 0)
                return "-";

            return $"{Min}-{Max}";
        }
    }

    public sealed class Skill
    {
        public int SkillId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Damage HandDamage { get; set; }
        public Damage SharpDamage { get; set; }
        public Damage BluntDamage { get; set; }

        public Damage GetDamage(Weapon weapon)
        {
            switch (weapon.DamageType)
            {
                case DamageType.Fists:
                    return HandDamage;
                case DamageType.Sharp:
                    return SharpDamage;
                case DamageType.Blunt:
                    return BluntDamage;
                default:
                    throw new InvalidOperationException("Invalid damage type.");
            }
        }
    }

    public enum DamageType
    {
        Fists = 1,
        Sharp = 2,
        Blunt = 3
    }

    public sealed class Weapon
    {
        public int WeaponId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal DamageModifier { get; set; }
        public DamageType DamageType { get; set; }
    }

    public static class Data
    {
        public static readonly string[] Texts = new[]
        {
            @"
You might be one of those people that perform very well when it comes to sports.
If you are and you are in high school, you might have the idea that your ability
at a given sport could end up putting you in line for a sports scholarship that
will either entirely pay for college, or at least partially pay for your higher
education. However, there is more to getting a scholarship than just being good
at sports. One thing you have to understand is that with sports scholarships
there are a few ways to get them.",
            @"
Before we begin, I would like to briefly explain why open source is important.
When we think of the software we use to write, most people think of programs
written by big corporations like Microsoft Word Scrivener. These programs cost
money and are built by large teams of programmers. At anytime these companies
and these products could go away and not be available anymore.
Open source programs are a little different. The vast majority are free. The
code used to create them is freely available, meaning that if the original
developer stops work on his project, someone else can take it up. It also means
that if you have some coding knowledge, you too can contribute to the project.
Open source developers typically respond much quicker to their users than huge
multinational organizations."
        };

        public static readonly Skill[] Skills = new[]
        {
            new Skill
            {
                SkillId = 1,
                Name = "Fist kick",
                Description = "Kick with bare fists. Rather ineffective, but it's the only way to fight without any weapon.",
                HandDamage = new Damage(1, 1)
            },
            new Skill
            {
                SkillId = 2,
                Name = "Stab",
                Description = "Stab with something sharp. Works with any melee weapon, however it's most useful with sharp weapons.",
                SharpDamage = new Damage(10, 25),
                BluntDamage = new Damage(5, 15)
            },
            new Skill
            {
                SkillId = 3,
                Name = "Smash",
                Description = "Smash with something blunt. Works with any melee weapon, however it's most useful with blunt weapons.",
                SharpDamage = new Damage(5, 15),
                BluntDamage = new Damage(10, 25)
            }
        };

        public static readonly Weapon[] Weapons = new[]
        {
            new Weapon
            {
                WeaponId = 1,
                Name = "Hunting knife",
                Description = "Rather good hunting knife. Can be used to leave deep cuts on somebody's flesh.",
                DamageModifier = 1,
                DamageType = DamageType.Sharp
            }
        };

        public static readonly PassZone[] PassZones = new[]
        {
            new PassZone
            {
                PassZoneId = 1,
                FromZoneId = 1,
                ToZoneId = 2,
                ForwardPassage = new Passage
                {
                    Name = "To training grounds",
                    Description = "HEALING ROAD. The road to the north leads downhill, to the training grounds. The passage is often used by soldiers so the road looks dirty and scabed. It's a short, safe passage to the place where everyone can test their skill in combat against a worthy opponent. The road goes downhill, rather easy walk.. but the way back will be laborous.",
                    Distance = 100,
                    HealingFactor = new HealingFactor(10, 5)
                },
                BackwardPassage = new Passage
                {
                    Name = "To the city",
                    Description = "DAMAGING ROAD. A crooked road goes to the south of here, climbing uphill to the castle. It will not be an easy walk, rather a laborous climb. But you knew that already, going down here.",
                    Distance = 150,
                    HealingFactor = new HealingFactor(1, -1)
                }
            },
            new PassZone
            {
                PassZoneId = 2,
                FromZoneId = 1,
                ToZoneId = 3,
                ForwardPassage = new Passage
                {
                    Name = "The grove",
                    Description = "Somewhat long road to the grove to the west of town leads to some shallow forest. Not too dangerous, but not too safe to roam either.",
                    Distance = 300
                },
                BackwardPassage = new Passage
                {
                    Name = "From the grove to the city",
                    Description = "From the grove to the city description",
                    Distance = 300
                }
            },
            new PassZone
            {
                PassZoneId = 3,
                FromZoneId = 2,
                ToZoneId = 3,
                ForwardPassage = new Passage
                {
                    Name = "To the grove",
                    Description = "To the grove desc",
                    Distance = 5
                },
                BackwardPassage = new Passage
                {
                    Name = "To the training ground",
                    Description = "training desc",
                    Distance = 5
                }
            }
        };

        public static readonly Zone[] Zones = new[]
        {
            new Zone
            {
                ZoneId = 1,
                Name = "Meadow village",
                Description = "Meadow village is a home to any traveler. It's pretty and beautiful. A place to relax and stay.",
                ZoneExits = new[] { 1, 2 }
            },
            new Zone
            {
                ZoneId = 2,
                Name = "Training ground",
                Description = "This is the place to train in combat. You can see the walls of the castle presiding over the valley to the south of here.",
                ZoneExits = new[] { 3 },
                ZoneEntrances = new[] { 1 }
            },
            new Zone
            {
                ZoneId = 3,
                Name = "The grove",
                Description = "Somewhat clear but not safe grove.",
                ZoneEntrances = new[] { 2, 3 }
            }
        };

        public static Skill GetSkill(int skillId)
        {
            return Skills.Single(s => s.SkillId == skillId);
        }

        public static Weapon GetWeapon(int weaponId)
        {
            return Weapons.Single(w => w.WeaponId == weaponId);
        }

        public static Zone GetZone(int zoneId)
        {
            return Zones.Single(z => z.ZoneId == zoneId);
        }

        public static PassZone GetPassZone(int passZoneId)
        {
            return PassZones.Single(z => z.PassZoneId == passZoneId);
        }

        public static string GetText(int length, int width)
        {
            var builder = new StringBuilder();
            foreach (var text in GetTexts())
            {
                builder.Append(text);

                if (builder.Length >= length) // +1 to count whitespace that is appended below.
                    break;

                builder.Append(" ");
            }

            var textBlock = builder.ToString().Substring(0, length);
            var splitBuilder = new StringBuilder();

            foreach (var line in WrapText(textBlock, 90))
            {
                splitBuilder.Append($"{line}\n");
            }

            return splitBuilder.ToString().Trim('\n');
        }

        public static IEnumerable<string> GetWords(int fromLength, int toLength)
        {
            foreach (var text in GetTexts())
            {
                var words = text.Split(' ')
                    .Distinct()
                    .Where(w => w.Length >= fromLength && w.Length <= toLength)
                    .Where(w => Regex.IsMatch(w, @"^[a-z]+$"))
                    .OrderBy(w => Randomizer.Next());

                foreach (var word in words)
                {
                    yield return word;
                }
            }
        }

        public static IEnumerable<string> GetPhrases(int fromLength, int toLength)
        {
            string previousWord = null;

            foreach (var word in GetWords(fromLength, toLength))
            {
                if (previousWord == null)
                {
                    previousWord = word;
                    continue;
                }

                var phrase = $"{previousWord} {word}";
                if (phrase.Length <= toLength)
                {
                    yield return phrase;
                    previousWord = null;
                    continue;
                }

                yield return previousWord;
                previousWord = word;
            }
        }

        private static IEnumerable<string> GetTexts()
        {
            var i = 0;
            var texts = Data.Texts.OrderBy(t => Randomizer.Next()).ToList();

            while (true)
            {
                yield return texts[i]
                    .Trim('\r').Trim('\n')
                    .Replace("\r\n", " ")
                    .Replace("\n", " ");

                i++;

                if (i == texts.Count)
                {
                    i = 0;
                    texts = Data.Texts.OrderBy(t => Randomizer.Next()).ToList();
                }
            }
        }

        // TODO: Duplicate code as in Program.cs in consoleapp.
        private static IEnumerable<string> WrapText(string text, int width)
        {
            while (text.Length > 0)
            {
                var part = text.Substring(0, Math.Min(width, text.Length));

                if (part.Contains(' ') && part != text)
                {
                    while (part[part.Length - 1] != ' ')
                    {
                        part = part.Substring(0, part.Length - 1);
                    }

                    part = part.Substring(0, part.Length - 1);
                    text = text.Substring(1);
                }

                text = text.Substring(part.Length);

                yield return part;
            }
        }
    }

    public sealed class Zone
    {
        public int ZoneId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int[] ZoneExits { get; set; } = new int[] { };
        public int[] ZoneEntrances { get; set; } = new int[] { };
    }

    public struct HealingFactor
    {
        public HealingFactor(int steps, int hp)
        {
            Steps = steps;
            Hp = hp;
        }

        public int Steps { get; }
        public int Hp { get; }

        public bool IsValid => Steps > 0 && Hp != 0;
    }

    public sealed class Passage
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Distance { get; set; }
        public HealingFactor HealingFactor { get; set; }
    }

    public sealed class PassageInfo
    {
        public PassageInfo(int fromZoneId, int toZoneId, Passage passage)
        {
            FromZoneId = fromZoneId;
            ToZoneId = toZoneId;
            Passage = passage;
        }

        public int FromZoneId { get; }
        public int ToZoneId { get; }
        public Passage Passage { get; }
    }

    public sealed class PassZone
    {
        public int PassZoneId { get; set; }
        public int FromZoneId { get; set; }
        public int ToZoneId { get; set; }

        public Passage ForwardPassage { get; set; }
        public Passage BackwardPassage { get; set; }

        /*public string ForwardName { get; set; }
        public string ForwardDescription { get; set; }
        public int ForwardDistance { get; set; }

        public string BackwardName { get; set; }
        public string BackwardDescription { get; set; }
        public int BackwardDistance { get; set; }*/

        public int GetFromZoneId(Messages.PassDirection direction)
        {
            return direction == Messages.PassDirection.Forward ? FromZoneId : ToZoneId;
        }

        public int GetToZoneId(Messages.PassDirection direction)
        {
            return direction == Messages.PassDirection.Forward ? ToZoneId : FromZoneId;
        }

        public string GetName(Messages.PassDirection direction)
        {
            return direction == Messages.PassDirection.Forward ? ForwardPassage.Name : BackwardPassage.Name;
        }

        public string GetDescription(Messages.PassDirection direction)
        {
            return direction == Messages.PassDirection.Forward ? ForwardPassage.Description : BackwardPassage.Description;
        }

        public int GetDistance(Messages.PassDirection direction)
        {
            return direction == Messages.PassDirection.Forward ? ForwardPassage.Distance : BackwardPassage.Distance;
        }

        public int GetReverseDistance(Messages.PassDirection direction)
        {
            return direction == Messages.PassDirection.Forward ? BackwardPassage.Distance : ForwardPassage.Distance;
        }

        public Passage GetPassage(PassDirection direction)
        {
            return direction == PassDirection.Forward ? ForwardPassage : BackwardPassage;
        }
    }

    /*public sealed class ZoneExit
    {
        public int ZoneId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Distance { get; set; }
        // Mobs info.
    }*/
}
