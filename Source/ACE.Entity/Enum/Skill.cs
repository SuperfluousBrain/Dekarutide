using System.Collections.Generic;
using System.Linq;

namespace ACE.Entity.Enum
{
    /// <summary>
    /// note: even though these are unnumbered, order is very important.  values of "none" or commented
    /// as retired or unused --ABSOLUTELY CANNOT-- be removed. Skills that are none, retired, or not
    /// implemented have been removed from the SkillHelper.ValidSkills hashset below.
    /// </summary>
    public enum Skill
    {
        None,
        Axe,                 /* Retired */
        Bow,                 /* Retired */
        Crossbow,            /* Retired */
        Dagger,              /* Retired */
        Mace,                /* Retired */
        MeleeDefense,
        MissileDefense,
        Sling,               /* Retired */
        Spear,               /* Retired */
        Staff,               /* Retired */
        Sword,               /* Retired */
        ThrownWeapon,        /* Retired */
        UnarmedCombat,       /* Retired */
        ArcaneLore,
        MagicDefense,
        ManaConversion,
        Spellcraft,          /* Unimplemented */
        ItemTinkering,
        AssessPerson,
        Deception,
        Healing,
        Jump,
        Lockpick,
        Run,
        Awareness,           /* Unimplemented */
        ArmsAndArmorRepair,  /* Unimplemented */
        AssessCreature,
        WeaponTinkering,
        ArmorTinkering,
        MagicItemTinkering,
        CreatureEnchantment,
        ItemEnchantment,
        LifeMagic,
        WarMagic,
        Leadership,
        Loyalty,
        Fletching,
        Alchemy,
        Cooking,
        Salvaging,
        TwoHandedCombat,
        Gearcraft,           /* Retired */
        VoidMagic,
        HeavyWeapons,
        LightWeapons,
        FinesseWeapons,
        MissileWeapons,
        Shield,
        DualWield,
        Recklessness,
        SneakAttack,
        DirtyFighting,
        Challenge,          /* Unimplemented */
        Summoning,
        // CustomDM
        Appraise,
        Armor,
        Sneaking
    }

    public static class SkillExtensions
    {
        public static List<Skill> RetiredMelee = new List<Skill>()
        {
            Skill.Axe,
            Skill.Dagger,
            Skill.Mace,
            Skill.Spear,
            Skill.Staff,
            Skill.Sword,
            Skill.UnarmedCombat
        };

        public static List<Skill> RetiredMissile = new List<Skill>()
        {
            Skill.Bow,
            Skill.Crossbow,
            Skill.Sling,
            Skill.ThrownWeapon
        };

        public static List<Skill> RetiredWeapons = RetiredMelee.Concat(RetiredMissile).ToList();

        /// <summary>
        /// Will add a space infront of capital letter words in a string
        /// </summary>
        /// <param name="skill"></param>
        /// <returns>string with spaces infront of capital letters</returns>
        public static string ToSentence(this Skill skill)
        {
            return new string(skill.ToString().ToCharArray().SelectMany((c, i) => i > 0 && char.IsUpper(c) ? new char[] { ' ', c } : new char[] { c }).ToArray());
        }
    }

    public static class SkillHelper
    {
        static SkillHelper()
        {
            if (Common.ConfigManager.Config.Server.WorldRuleset == Common.Ruleset.Infiltration)
            {
                ValidSkills.Remove(Skill.TwoHandedCombat);
                ValidSkills.Remove(Skill.HeavyWeapons);
                ValidSkills.Remove(Skill.LightWeapons);
                ValidSkills.Remove(Skill.FinesseWeapons);
                ValidSkills.Remove(Skill.MissileWeapons);
                ValidSkills.Remove(Skill.Shield);
                ValidSkills.Remove(Skill.DualWield);
                ValidSkills.Remove(Skill.Recklessness);
                ValidSkills.Remove(Skill.SneakAttack);
                ValidSkills.Remove(Skill.DirtyFighting);
                ValidSkills.Remove(Skill.VoidMagic);
                ValidSkills.Remove(Skill.Summoning);

                ValidSkills.Add(Skill.Axe);
                ValidSkills.Add(Skill.Bow);
                ValidSkills.Add(Skill.Crossbow);
                ValidSkills.Add(Skill.Dagger);
                ValidSkills.Add(Skill.Mace);
                ValidSkills.Add(Skill.Spear);
                ValidSkills.Add(Skill.Staff);
                ValidSkills.Add(Skill.Sword);
                ValidSkills.Add(Skill.ThrownWeapon);
                ValidSkills.Add(Skill.UnarmedCombat);
                ValidSkills.Add(Skill.Salvaging);
            }
            else if (Common.ConfigManager.Config.Server.WorldRuleset == Common.Ruleset.CustomDM)
            {
                ValidSkills.Remove(Skill.TwoHandedCombat);
                ValidSkills.Remove(Skill.HeavyWeapons);
                ValidSkills.Remove(Skill.LightWeapons);
                ValidSkills.Remove(Skill.FinesseWeapons);
                ValidSkills.Remove(Skill.MissileWeapons);
                ValidSkills.Remove(Skill.Recklessness);
                ValidSkills.Remove(Skill.SneakAttack);
                ValidSkills.Remove(Skill.DirtyFighting);
                ValidSkills.Remove(Skill.VoidMagic);
                ValidSkills.Remove(Skill.Summoning);

                ValidSkills.Remove(Skill.AssessPerson);
                ValidSkills.Remove(Skill.ItemEnchantment);
                ValidSkills.Remove(Skill.CreatureEnchantment);
                ValidSkills.Remove(Skill.Crossbow);
                ValidSkills.Remove(Skill.Mace);
                ValidSkills.Remove(Skill.Staff);

                ValidSkills.Add(Skill.Axe);
                ValidSkills.Add(Skill.Bow);
                ValidSkills.Add(Skill.Crossbow);
                ValidSkills.Add(Skill.Dagger);
                ValidSkills.Add(Skill.Mace);
                ValidSkills.Add(Skill.Spear);
                ValidSkills.Add(Skill.Staff);
                ValidSkills.Add(Skill.Sword);
                ValidSkills.Add(Skill.ThrownWeapon);
                ValidSkills.Add(Skill.UnarmedCombat);
                ValidSkills.Add(Skill.Salvaging);

                ValidSkills.Add(Skill.Awareness);
                ValidSkills.Add(Skill.Appraise);
                ValidSkills.Add(Skill.Armor);
                ValidSkills.Add(Skill.Sneaking);
            }
        }

        public static HashSet<Skill> ValidSkills = new HashSet<Skill>
        {
            Skill.MeleeDefense,
            Skill.MissileDefense,
            Skill.ArcaneLore,
            Skill.MagicDefense,
            Skill.ManaConversion,
            Skill.ItemTinkering,
            Skill.AssessPerson,
            Skill.Deception,
            Skill.Healing,
            Skill.Jump,
            Skill.Lockpick,
            Skill.Run,
            Skill.AssessCreature,
            Skill.WeaponTinkering,
            Skill.ArmorTinkering,
            Skill.MagicItemTinkering,
            Skill.CreatureEnchantment,
            Skill.ItemEnchantment,
            Skill.LifeMagic,
            Skill.WarMagic,
            Skill.Leadership,
            Skill.Loyalty,
            Skill.Fletching,
            Skill.Alchemy,
            Skill.Cooking,
            Skill.Salvaging,
            Skill.TwoHandedCombat,
            Skill.VoidMagic,
            Skill.HeavyWeapons,
            Skill.LightWeapons,
            Skill.FinesseWeapons,
            Skill.MissileWeapons,
            Skill.Shield,
            Skill.DualWield,
            Skill.Recklessness,
            Skill.SneakAttack,
            Skill.DirtyFighting,
            Skill.Summoning
        };

        public static HashSet<Skill> AttackSkills = new HashSet<Skill>
        {
            Skill.Axe,
            Skill.Bow,
            Skill.Crossbow,
            Skill.Dagger,
            Skill.Mace,
            Skill.Sling,
            Skill.Spear,
            Skill.Staff,
            Skill.Sword,
            Skill.ThrownWeapon,
            Skill.UnarmedCombat,
            Skill.FinesseWeapons,
            Skill.HeavyWeapons,
            Skill.LightWeapons,
            Skill.MissileWeapons,
            Skill.TwoHandedCombat,
            Skill.WarMagic,
            Skill.LifeMagic,
            Skill.VoidMagic,
            Skill.DualWield,
            //Skill.Recklessness,   // confirmed not in client
            //Skill.DirtyFighting,
            //Skill.SneakAttack
        };

        public static HashSet<Skill> DefenseSkills = new HashSet<Skill>()
        {
            Skill.MeleeDefense,
            Skill.MissileDefense,
            Skill.MagicDefense,
            Skill.Shield            // confirmed in client
        };
    }
}
