DELETE FROM `weenie` WHERE `class_Id` = 30687;

INSERT INTO `weenie` (`class_Id`, `class_Name`, `type`, `last_Modified`)
VALUES (30687, 'tumerokreveredshamanwithered', 10, '2019-09-13 00:00:00') /* Creature */;

INSERT INTO `weenie_properties_int` (`object_Id`, `type`, `value`)
VALUES (30687,   1,         16) /* ItemType - Creature */
     , (30687,   2,          6) /* CreatureType - Tumerok */
     , (30687,   3,         39) /* PaletteTemplate - Black */
     , (30687,   6,         -1) /* ItemsCapacity */
     , (30687,   7,         -1) /* ContainersCapacity */
     , (30687,  16,          1) /* ItemUseable - No */
     , (30687,  25,        161) /* Level */
     , (30687,  27,          0) /* ArmorType - None */
     , (30687,  68,          5) /* TargetingTactic - Random, LastDamager */
     , (30687,  72,          6) /* FriendType - Tumerok */
     , (30687,  93,       1032) /* PhysicsState - ReportCollisions, Gravity */
     , (30687, 101,        183) /* AiAllowedCombatStyle - Unarmed, OneHanded, OneHandedAndShield, Bow, Crossbow, ThrownWeapon */
     , (30687, 133,          2) /* ShowableOnRadar - ShowMovement */
     , (30687, 140,          1) /* AiOptions - CanOpenDoors */
     , (30687, 146,     243771) /* XpOverride */;

INSERT INTO `weenie_properties_bool` (`object_Id`, `type`, `value`)
VALUES (30687,   1, True ) /* Stuck */
     , (30687,   6, True ) /* AiUsesMana */
     , (30687,  11, False) /* IgnoreCollisions */
     , (30687,  12, True ) /* ReportCollisions */
     , (30687,  13, False) /* Ethereal */
     , (30687,  50, True ) /* NeverFailCasting */
     , (30687, 103, True ) /* NonProjectileMagicImmune */
     , (30687, 9016, True) /* UseXpOverride */;

INSERT INTO `weenie_properties_float` (`object_Id`, `type`, `value`)
VALUES (30687,   1,       5) /* HeartbeatInterval */
     , (30687,   2,       0) /* HeartbeatTimestamp */
     , (30687,   3,       6) /* HealthRate */
     , (30687,   4,      10) /* StaminaRate */
     , (30687,   5,       5) /* ManaRate */
     , (30687,  12,     0.5) /* Shade */
     , (30687,  13,       1) /* ArmorModVsSlash */
     , (30687,  14,       1) /* ArmorModVsPierce */
     , (30687,  15,       1) /* ArmorModVsBludgeon */
     , (30687,  16,       1) /* ArmorModVsCold */
     , (30687,  17,       1) /* ArmorModVsFire */
     , (30687,  18,       1) /* ArmorModVsAcid */
     , (30687,  19,       1) /* ArmorModVsElectric */
     , (30687,  31,      16) /* VisualAwarenessRange */
     , (30687,  34,       1) /* PowerupTime */
     , (30687,  36,       1) /* ChargeSpeed */
     , (30687,  39,     1.3) /* DefaultScale */
     , (30687,  64,       2) /* ResistSlash */
     , (30687,  65,       1) /* ResistPierce */
     , (30687,  66,       1) /* ResistBludgeon */
     , (30687,  67,       1) /* ResistFire */
     , (30687,  68,       1) /* ResistCold */
     , (30687,  69,       1) /* ResistAcid */
     , (30687,  70,       1) /* ResistElectric */
     , (30687,  71,       1) /* ResistHealthBoost */
     , (30687,  72,       1) /* ResistStaminaDrain */
     , (30687,  73,       1) /* ResistStaminaBoost */
     , (30687,  74,       1) /* ResistManaDrain */
     , (30687,  75,       1) /* ResistManaBoost */
     , (30687,  80,       3) /* AiUseMagicDelay */
     , (30687, 104,      10) /* ObviousRadarRange */
     , (30687, 125,       1) /* ResistHealthDrain */;

INSERT INTO `weenie_properties_string` (`object_Id`, `type`, `value`)
VALUES (30687,   1, 'Withered Revered Tumerok Shaman') /* Name */;

INSERT INTO `weenie_properties_d_i_d` (`object_Id`, `type`, `value`)
VALUES (30687,   1, 0x020012B1) /* Setup */
     , (30687,   2, 0x0900000A) /* MotionTable */
     , (30687,   3, 0x20000013) /* SoundTable */
     , (30687,   4, 0x3000000C) /* CombatTable */
     , (30687,   6, 0x040001C2) /* PaletteBase */
     , (30687,   7, 0x100005A3) /* ClothingBase */
     , (30687,   8, 0x0600103C) /* Icon */
     , (30687,  22, 0x34000026) /* PhysicsEffectTable */
     , (30687,  32,        490) /* WieldedTreasureType */
     , (30687,  35,        449) /* DeathTreasureType - T6_General - Loot Tier: 6 */;

INSERT INTO `weenie_properties_attribute` (`object_Id`, `type`, `init_Level`, `level_From_C_P`, `c_P_Spent`)
VALUES (30687,   1, 260, 0, 0) /* Strength */
     , (30687,   2, 300, 0, 0) /* Endurance */
     , (30687,   3, 325, 0, 0) /* Quickness */
     , (30687,   4, 300, 0, 0) /* Coordination */
     , (30687,   5, 320, 0, 0) /* Focus */
     , (30687,   6, 350, 0, 0) /* Self */;

INSERT INTO `weenie_properties_attribute_2nd` (`object_Id`, `type`, `init_Level`, `level_From_C_P`, `c_P_Spent`, `current_Level`)
VALUES (30687,   1,  4850, 0, 0, 5000) /* MaxHealth */
     , (30687,   3,  4700, 0, 0, 5000) /* MaxStamina */
     , (30687,   5,  4650, 0, 0, 5000) /* MaxMana */;

INSERT INTO `weenie_properties_skill` (`object_Id`, `type`, `level_From_P_P`, `s_a_c`, `p_p`, `init_Level`, `resistance_At_Last_Check`, `last_Used_Time`)
VALUES (30687,  2, 0, 3, 0, 240, 0, 0) /* Bow                 Specialized */
     , (30687,  4, 0, 3, 0, 285, 0, 0) /* Dagger              Specialized */
     , (30687,  6, 0, 3, 0, 320, 0, 0) /* MeleeDefense        Specialized */
     , (30687,  7, 0, 3, 0, 430, 0, 0) /* MissileDefense      Specialized */
     , (30687, 14, 0, 3, 0, 300, 0, 0) /* ArcaneLore          Specialized */
     , (30687, 15, 0, 3, 0, 295, 0, 0) /* MagicDefense        Specialized */
     , (30687, 20, 0, 3, 0, 150, 0, 0) /* Deception           Specialized */
     , (30687, 24, 0, 3, 0,  60, 0, 0) /* Run                 Specialized */
     , (30687, 31, 0, 3, 0, 230, 0, 0) /* CreatureEnchantment Specialized */
     , (30687, 33, 0, 3, 0, 230, 0, 0) /* LifeMagic           Specialized */
     , (30687, 34, 0, 3, 0, 230, 0, 0) /* WarMagic            Specialized */;

INSERT INTO `weenie_properties_body_part` (`object_Id`, `key`, `d_Type`, `d_Val`, `d_Var`, `base_Armor`, `armor_Vs_Slash`, `armor_Vs_Pierce`, `armor_Vs_Bludgeon`, `armor_Vs_Cold`, `armor_Vs_Fire`, `armor_Vs_Acid`, `armor_Vs_Electric`, `armor_Vs_Nether`, `b_h`, `h_l_f`, `m_l_f`, `l_l_f`, `h_r_f`, `m_r_f`, `l_r_f`, `h_l_b`, `m_l_b`, `l_l_b`, `h_r_b`, `m_r_b`, `l_r_b`)
VALUES (30687,  0,  4,  0,    0,  450,  450,  450,  450,  450,  450,  450,  450,    0, 1, 0.33,    0,    0, 0.33,    0,    0, 0.33,    0,    0, 0.33,    0,    0) /* Head */
     , (30687,  1,  4,  0,    0,  450,  450,  450,  450,  450,  450,  450,  450,    0, 2, 0.44, 0.17,    0, 0.44, 0.17,    0, 0.44, 0.17,    0, 0.44, 0.17,    0) /* Chest */
     , (30687,  2,  4,  0,    0,  450,  450,  450,  450,  450,  450,  450,  450,    0, 3,    0, 0.17,    0,    0, 0.17,    0,    0, 0.17,    0,    0, 0.17,    0) /* Abdomen */
     , (30687,  3,  4,  0,    0,  450,  450,  450,  450,  450,  450,  450,  450,    0, 1, 0.23, 0.03,    0, 0.23, 0.03,    0, 0.23, 0.03,    0, 0.23, 0.03,    0) /* UpperArm */
     , (30687,  4,  4,  0,    0,  450,  450,  450,  450,  450,  450,  450,  450,    0, 2,    0,  0.3,    0,    0,  0.3,    0,    0,  0.3,    0,    0,  0.3,    0) /* LowerArm */
     , (30687,  5,  4, 50, 0.75,  450,  450,  450,  450,  450,  450,  450,  450,    0, 2,    0,  0.2,    0,    0,  0.2,    0,    0,  0.2,    0,    0,  0.2,    0) /* Hand */
     , (30687,  6,  4,  0,    0,  450,  450,  450,  450,  450,  450,  450,  450,    0, 3,    0, 0.13, 0.18,    0, 0.13, 0.18,    0, 0.13, 0.18,    0, 0.13, 0.18) /* UpperLeg */
     , (30687,  7,  4,  0,    0,  450,  450,  450,  450,  450,  450,  450,  450,    0, 3,    0,    0,  0.6,    0,    0,  0.6,    0,    0,  0.6,    0,    0,  0.6) /* LowerLeg */
     , (30687,  8,  4, 55, 0.75,  450,  450,  450,  450,  450,  450,  450,  450,    0, 3,    0,    0, 0.22,    0,    0, 0.22,    0,    0, 0.22,    0,    0, 0.22) /* Foot */;

INSERT INTO `weenie_properties_spell_book` (`object_Id`, `spell`, `probability`)
VALUES (30687,    63,  2.015)  /* Acid Stream VI */
     , (30687,    69,  2.015)  /* Shock Wave VI */
     , (30687,    74,  2.015)  /* Frost Bolt VI */
     , (30687,    80,  2.015)  /* Lightning Bolt VI */
     , (30687,    85,  2.015)  /* Flame Bolt VI */
     , (30687,    91,  2.015)  /* Force Bolt VI */
     , (30687,    97,  2.015)  /* Whirling Blade VI */
     , (30687,   106,  2.015)  /* Shock Blast VI */
     , (30687,   138,  2.015)  /* Frost Volley VI */
     , (30687,   142,  2.015)  /* Lightning Volley VI */
     , (30687,   146,  2.015)  /* Flame Volley VI */
     , (30687,   154,  2.015)  /* Blade Volley VI */
     , (30687,   234,  2.012)  /* Vulnerability Other VI */
     , (30687,   267,  2.012)  /* Defenselessness Other VI */
     , (30687,   285,  2.012)  /* Magic Yield Other VI */
     , (30687,  1161,  2.009)  /* Heal Self VI */
     , (30687,  1176,  2.012)  /* Harm Other VI */
     , (30687,  1200,  2.012)  /* Enfeeble Other VI */
     , (30687,  1265,  2.012)  /* Drain Mana Other VI */
     , (30687,  1468,  2.012)  /* Feeblemind Other VI */;

INSERT INTO `weenie_properties_event_filter` (`object_Id`, `event`)
VALUES (30687,  94) /* ATTACK_NOTIFICATION_EVENT */
     , (30687, 414) /* PLAYER_DEATH_EVENT */;

INSERT INTO `weenie_properties_create_list` (`object_Id`, `destination_Type`, `weenie_Class_Id`, `stack_Size`, `palette`, `shade`, `try_To_Bond`)
VALUES (30687, 9, 23108,  0, 0, 0.02, False) /* Create Twisted Dark Key (23108) for ContainTreasure */
     , (30687, 9,     0,  0, 0, 0.98, False) /* Create nothing for ContainTreasure */
     , (30687, 9, 23107,  0, 0, 0.01, False) /* Create Mangled Dark Key (23107) for ContainTreasure */
     , (30687, 9,     0,  0, 0, 0.99, False) /* Create nothing for ContainTreasure */;
