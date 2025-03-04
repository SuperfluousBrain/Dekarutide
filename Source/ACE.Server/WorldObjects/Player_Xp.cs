using System;
using System.Linq;

using ACE.Common.Extensions;
using ACE.DatLoader;
using ACE.Entity.Enum;
using ACE.Entity.Enum.Properties;
using ACE.Server.Command.Handlers;
using ACE.Server.Entity.Actions;
using ACE.Server.Managers;
using ACE.Server.Network.GameMessages.Messages;

namespace ACE.Server.WorldObjects
{
    partial class Player
    {
        /// <summary>
        /// A player earns XP through natural progression, ie. kills and quests completed
        /// </summary>
        /// <param name="amount">The amount of XP being added</param>
        /// <param name="xpType">The source of XP being added</param>
        /// <param name="shareable">True if this XP can be shared with Fellowship</param>
        public void EarnXP(long amount, XpType xpType, int? xpSourceLevel, uint? xpSourceId, ShareType shareType = ShareType.All)
        {
            //Console.WriteLine($"{Name}.EarnXP({amount}, {sharable}, {fixedAmount})");

            string xpMessage = "";
            bool usesRewardByLevelSystem = false;
            int formulaVersion = 0;
            if (xpType == XpType.Quest && amount < 0 && amount > -6000) // this range is used to specify the reward by level system.
            {
                // The following comments are just recommendations and vary from quest to quest, but the larger the value the higher the xp sum awarded.
                if (amount <= -5000) // once per character
                {
                    xpSourceLevel = -((int)amount + 5000);
                    formulaVersion = 5;
                }
                else if (amount <= -4000) // once every 3 weeks or more
                {
                    xpSourceLevel = -((int)amount + 4000);
                    formulaVersion = 4;
                }
                else if (amount <= -3000) // once a week or more
                {
                    xpSourceLevel = -((int)amount + 3000);
                    formulaVersion = 3;
                }
                else if (amount <= -2000) // once a day or more
                {
                    xpSourceLevel = -((int)amount + 2000);
                    formulaVersion = 2;
                }
                else if (amount <= -1000) // more than once per day
                {
                    xpSourceLevel = -((int)amount + 1000);
                    formulaVersion = 1;
                }
                else
                {
                    xpSourceLevel = -(int)amount;
                    formulaVersion = 0;
                }
                usesRewardByLevelSystem = true;

                int modifiedLevel = Math.Max((int)Level, 5);

                if (Level < 100 && modifiedLevel <= xpSourceLevel / 3)
                {
                    xpSourceLevel = modifiedLevel * 3;
                    Session.Network.EnqueueSend(new GameMessageSystemChat("Your experience reward has been reduced because your level is not high enough!", ChatMessageType.System));
                }

                float totalXP = Creature.GetCreatureDeathXP(xpSourceLevel.Value, 0, 0, formulaVersion);

                if (xpSourceId != null && xpSourceId != 0)
                {
                    float typeCampBonus;
                    CampManager.HandleCampInteraction(xpSourceId.Value ^ 0xFFFF0000, null, out typeCampBonus, out _, out _);

                    totalXP = totalXP * typeCampBonus;

                    xpMessage = $"T: {(typeCampBonus * 100).ToString("0")}%";
                }

                amount = (long)Math.Round(totalXP);
            }
            else if (amount < 0)
            {
                SpendXP(-amount);
                return;
            }
            else if (xpType == XpType.Kill && Common.ConfigManager.Config.Server.WorldRuleset == Common.Ruleset.CustomDM)
            {
                float typeCampBonus;
                float areaCampBonus;
                float restCampBonus;

                float totalXP = amount;

                if (xpSourceId != null && xpSourceId != 0)
                {
                    CampManager.HandleCampInteraction(xpSourceId.Value, CurrentLandblock, out typeCampBonus, out areaCampBonus, out restCampBonus);

                    float thirdXP = totalXP / 3.0f;
                    totalXP = (thirdXP * typeCampBonus) + (thirdXP * areaCampBonus) + (thirdXP * restCampBonus);

                    xpMessage = $"T: {(typeCampBonus * 100).ToString("0")}% A: {(areaCampBonus * 100).ToString("0")}% R: {(restCampBonus * 100).ToString("0")}%";
                }

                if (CurrentLandblock != null && !(CurrentLandblock.IsDungeon || (CurrentLandblock.HasDungeon && Location.Indoors)))
                    totalXP *= 1.25f; // Surface provides 25% xp bonus to account for lower creature density.

                amount = (long)Math.Round(totalXP);
            }

            // apply xp modifier
            var modifier = PropertyManager.GetDouble("xp_modifier").Item;

            if (xpType == XpType.Kill && xpSourceLevel != null)
            {
                if (xpSourceLevel < 28)
                    modifier *= PropertyManager.GetDouble("xp_modifier_kill_tier1").Item;
                else if (xpSourceLevel < 65)
                    modifier *= PropertyManager.GetDouble("xp_modifier_kill_tier2").Item;
                else if (xpSourceLevel < 95)
                    modifier *= PropertyManager.GetDouble("xp_modifier_kill_tier3").Item;
                else if (xpSourceLevel < 110)
                    modifier *= PropertyManager.GetDouble("xp_modifier_kill_tier4").Item;
                else if (xpSourceLevel < 135)
                    modifier *= PropertyManager.GetDouble("xp_modifier_kill_tier5").Item;
                else
                    modifier *= PropertyManager.GetDouble("xp_modifier_kill_tier6").Item;
            }
            else if (xpType == XpType.Quest && xpSourceLevel != null)
            {
                if (usesRewardByLevelSystem)
                {
                    if (xpSourceLevel < 28)
                        modifier *= PropertyManager.GetDouble("xp_modifier_reward_tier1").Item;
                    else if (xpSourceLevel < 65)
                        modifier *= PropertyManager.GetDouble("xp_modifier_reward_tier2").Item;
                    else if (xpSourceLevel < 95)
                        modifier *= PropertyManager.GetDouble("xp_modifier_reward_tier3").Item;
                    else if (xpSourceLevel < 110)
                        modifier *= PropertyManager.GetDouble("xp_modifier_reward_tier4").Item;
                    else if (xpSourceLevel < 135)
                        modifier *= PropertyManager.GetDouble("xp_modifier_reward_tier5").Item;
                    else
                        modifier *= PropertyManager.GetDouble("xp_modifier_reward_tier6").Item;
                }
                else
                {
                    if (xpSourceLevel < 16)
                        modifier *= PropertyManager.GetDouble("xp_modifier_reward_tier1").Item;
                    else if (xpSourceLevel < 36)
                        modifier *= PropertyManager.GetDouble("xp_modifier_reward_tier2").Item;
                    else if (xpSourceLevel < 56)
                        modifier *= PropertyManager.GetDouble("xp_modifier_reward_tier3").Item;
                    else if (xpSourceLevel < 76)
                        modifier *= PropertyManager.GetDouble("xp_modifier_reward_tier4").Item;
                    else if (xpSourceLevel < 96)
                        modifier *= PropertyManager.GetDouble("xp_modifier_reward_tier5").Item;
                    else
                        modifier *= PropertyManager.GetDouble("xp_modifier_reward_tier6").Item;
                }
            }

            // should this be passed upstream to fellowship / allegiance?
            var enchantment = GetXPAndLuminanceModifier(xpType);

            var m_amount = (long)Math.Round(amount * enchantment * modifier);

            if (m_amount < 0)
            {
                log.Warn($"{Name}.EarnXP({amount}, {shareType})");
                log.Warn($"modifier: {modifier}, enchantment: {enchantment}, m_amount: {m_amount}");
                return;
            }

            GrantXP(m_amount, xpType, shareType, xpMessage);
        }

        /// <summary>
        /// Directly grants XP to the player, without the XP modifier
        /// </summary>
        /// <param name="amount">The amount of XP to grant to the player</param>
        /// <param name="xpType">The source of the XP being granted</param>
        /// <param name="shareable">If TRUE, this XP can be shared with fellowship members</param>
        public void GrantXP(long amount, XpType xpType, ShareType shareType = ShareType.All, string xpMessage = "")
        {
            if (IsOlthoiPlayer)
            {
                if (HasVitae)
                    UpdateXpVitae(amount);

                return;
            }

            if (Fellowship != null && Fellowship.ShareXP && shareType.HasFlag(ShareType.Fellowship))
            {
                // this will divy up the XP, and re-call this function
                // with ShareType.Fellowship removed
                Fellowship.SplitXp((ulong)amount, xpType, shareType, this, xpMessage);
                return;
            }

            // Make sure UpdateXpAndLevel is done on this players thread
            EnqueueAction(new ActionEventDelegate(() => UpdateXpAndLevel(amount, xpType, xpMessage)));

            //Update XP tracking info
            try
            {
                if (!XpTrackerStartTimestamp.HasValue)
                {
                    XpTrackerStartTimestamp = ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds();
                    XpTrackerTotalXp = 0;
                }

                XpTrackerTotalXp += amount;
            }
            catch (Exception ex)
            {
                log.Error($"Exception in Player.GrantXP while updating XP tracking info. Ex: {ex}");
            }

            // for passing XP up the allegiance chain,
            // this function is only called at the very beginning, to start the process.
            if (shareType.HasFlag(ShareType.Allegiance))
                UpdateXpAllegiance(amount);

            // only certain types of XP are granted to items
            if (xpType == XpType.Kill || xpType == XpType.Quest)
                GrantItemXP(amount);
        }

        /// <summary>
        /// Adds XP to a player's total XP, handles triggers (vitae, level up)
        /// </summary>
        private void UpdateXpAndLevel(long amount, XpType xpType, string xpMessage = "")
        {
            // until we are max level we must make sure that we send
            var xpTable = DatManager.PortalDat.XpTable;

            var maxLevel = GetMaxLevel();
            var maxLevelXp = xpTable.CharacterLevelXPList[(int)maxLevel];

            bool allowXpAtMaxLevel = PropertyManager.GetBool("allow_xp_at_max_level").Item;
            var totalXpCap = (Common.ConfigManager.Config.Server.WorldRuleset == Common.Ruleset.EoR ? long.MaxValue : maxLevelXp); // 0 disables the xp cap
            var availableXpCap = (Common.ConfigManager.Config.Server.WorldRuleset == Common.Ruleset.EoR ? long.MaxValue : uint.MaxValue); // 0 disables the xp cap

            if (Level != maxLevel || allowXpAtMaxLevel)
            {
                var addAmount = amount;

                var amountLeftToEnd = (long)maxLevelXp - TotalExperience ?? 0;
                if (!allowXpAtMaxLevel && amount > amountLeftToEnd)
                    addAmount = amountLeftToEnd;

                TotalExperience += addAmount;
                if (totalXpCap > 0 && TotalExperience > (long)totalXpCap)
                    TotalExperience = (long)totalXpCap;

                AvailableExperience += addAmount;
                if (availableXpCap > 0 && AvailableExperience > (long)availableXpCap)
                    AvailableExperience = (long)availableXpCap;

                var xpTotalUpdate = new GameMessagePrivateUpdatePropertyInt64(this, PropertyInt64.TotalExperience, TotalExperience ?? 0);
                var xpAvailUpdate = new GameMessagePrivateUpdatePropertyInt64(this, PropertyInt64.AvailableExperience, AvailableExperience ?? 0);
                Session.Network.EnqueueSend(xpTotalUpdate, xpAvailUpdate);

                CheckForLevelup();
            }

            if (xpType == XpType.Quest)
                Session.Network.EnqueueSend(new GameMessageSystemChat($"You've earned {amount:N0} experience. {xpMessage}", ChatMessageType.Broadcast));
            else if (Common.ConfigManager.Config.Server.WorldRuleset == Common.Ruleset.CustomDM)
            {
                if (xpType == XpType.Fellowship)
                    Session.Network.EnqueueSend(new GameMessageSystemChat($"Your fellowship shared {amount:N0} experience with you!", ChatMessageType.Broadcast));
                else if (xpType == XpType.Kill && xpMessage != "")
                    Session.Network.EnqueueSend(new GameMessageSystemChat($"You've earned {amount:N0} experience! {xpMessage}", ChatMessageType.Broadcast));
                else if (amount > 0 && xpType == XpType.Proficiency && xpMessage != "")
                    Session.Network.EnqueueSend(new GameMessageSystemChat($"You've earned {amount:N0} {xpMessage} experience!", ChatMessageType.Broadcast));
            }

            if (HasVitae && xpType != XpType.Allegiance)
                UpdateXpVitae(amount);
        }

        /// <summary>
        /// Optionally passes XP up the Allegiance tree
        /// </summary>
        private void UpdateXpAllegiance(long amount)
        {
            if (!HasAllegiance) return;

            AllegianceManager.PassXP(AllegianceNode, (ulong)amount, true);
        }

        /// <summary>
        /// Handles updating the vitae penalty through earned XP
        /// </summary>
        /// <param name="amount">The amount of XP to apply to the vitae penalty</param>
        private void UpdateXpVitae(long amount)
        {
            var vitae = EnchantmentManager.GetVitae();

            if (vitae == null)
            {
                log.Error($"{Name}.UpdateXpVitae({amount}) vitae null, likely due to cross-thread operation or corrupt EnchantmentManager cache. Please report this.");
                log.Error(Environment.StackTrace);
                return;
            }

            var vitaePenalty = vitae.StatModValue;
            var startPenalty = vitaePenalty;

            var maxPool = (int)VitaeCPPoolThreshold(vitaePenalty, DeathLevel.Value);
            var curPool = VitaeCpPool + amount;
            while (curPool >= maxPool)
            {
                curPool -= maxPool;
                vitaePenalty = EnchantmentManager.ReduceVitae();
                if (vitaePenalty == 1.0f)
                    break;
                maxPool = (int)VitaeCPPoolThreshold(vitaePenalty, DeathLevel.Value);
            }
            VitaeCpPool = (int)curPool;

            Session.Network.EnqueueSend(new GameMessagePrivateUpdatePropertyInt(this, PropertyInt.VitaeCpPool, VitaeCpPool.Value));

            if (vitaePenalty != startPenalty)
            {
                Session.Network.EnqueueSend(new GameMessageSystemChat("Your experience has reduced your Vitae penalty!", ChatMessageType.Magic));
                EnchantmentManager.SendUpdateVitae();
            }

            if (vitaePenalty.EpsilonEquals(1.0f) || vitaePenalty > 1.0f)
            {
                var actionChain = new ActionChain();
                actionChain.AddDelaySeconds(2.0f);
                actionChain.AddAction(this, () =>
                {
                    var vitae = EnchantmentManager.GetVitae();
                    if (vitae != null)
                    {
                        var curPenalty = vitae.StatModValue;
                        if (curPenalty.EpsilonEquals(1.0f) || curPenalty > 1.0f)
                            EnchantmentManager.RemoveVitae();
                    }
                });
                actionChain.EnqueueChain();
            }
        }

        /// <summary>
        /// Returns the maximum possible character level
        /// </summary>
        public static uint GetMaxLevel()
        {
            uint maxPossibleLevel = (uint)DatManager.PortalDat.XpTable.CharacterLevelXPList.Count - 1;
            uint maxSettingLevel = (uint)PropertyManager.GetLong("max_level").Item;
            return (Math.Min(maxPossibleLevel, maxSettingLevel));
        }

        /// <summary>
        /// Returns TRUE if player >= MaxLevel
        /// </summary>
        public bool IsMaxLevel => Level >= GetMaxLevel();

        /// <summary>
        /// Returns the remaining XP required to reach a level
        /// </summary>
        public long? GetRemainingXP(uint level)
        {
            var maxLevel = GetMaxLevel();
            if (level < 1 || level > maxLevel)
                return null;

            var levelTotalXP = DatManager.PortalDat.XpTable.CharacterLevelXPList[(int)level];

            return (long)levelTotalXP - TotalExperience.Value;
        }

        /// <summary>
        /// Returns the remaining XP required to the next level
        /// </summary>
        public ulong GetRemainingXP()
        {
            var maxLevel = GetMaxLevel();
            if (Level >= maxLevel)
                return 0;

            var nextLevelTotalXP = DatManager.PortalDat.XpTable.CharacterLevelXPList[Level.Value + 1];
            return nextLevelTotalXP - (ulong)TotalExperience.Value;
        }

        /// <summary>
        /// Returns the total XP required to reach a level
        /// </summary>
        public static ulong GetTotalXP(int level)
        {
            var maxLevel = GetMaxLevel();
            if (level < 0 || level > maxLevel)
                return 0;

            return DatManager.PortalDat.XpTable.CharacterLevelXPList[level];
        }

        /// <summary>
        /// Returns the total amount of XP required for a player reach max level
        /// </summary>
        public static long MaxLevelXP
        {
            get
            {
                var xpTable = DatManager.PortalDat.XpTable.CharacterLevelXPList;

                return (long)xpTable[xpTable.Count - 1];
            }
        }

        /// <summary>
        /// Returns the XP required to go from level A to level B
        /// </summary>
        public ulong GetXPBetweenLevels(int levelA, int levelB)
        {
            // special case for max level
            var maxLevel = (int)GetMaxLevel();

            levelA = Math.Clamp(levelA, 1, maxLevel - 1);
            levelB = Math.Clamp(levelB, 1, maxLevel);

            var levelA_totalXP = DatManager.PortalDat.XpTable.CharacterLevelXPList[levelA];
            var levelB_totalXP = DatManager.PortalDat.XpTable.CharacterLevelXPList[levelB];

            return levelB_totalXP - levelA_totalXP;
        }

        public ulong GetXPToNextLevel(int level)
        {
            return GetXPBetweenLevels(level, level + 1);
        }

        /// <summary>
        /// Determines if the player has advanced a level
        /// </summary>
        private void CheckForLevelup()
        {
            var xpTable = DatManager.PortalDat.XpTable;

            var maxLevel = GetMaxLevel();

            if (Level >= maxLevel) return;

            var startingLevel = Level;
            bool creditEarned = false;

            // increases until the correct level is found
            while ((ulong)(TotalExperience ?? 0) >= xpTable.CharacterLevelXPList[(Level ?? 0) + 1])
            {
                Level++;

                // increase the skill credits if the chart allows this level to grant a credit
                if (xpTable.CharacterLevelSkillCreditList[Level ?? 0] > 0)
                {
                    AvailableSkillCredits += (int)xpTable.CharacterLevelSkillCreditList[Level ?? 0];
                    TotalSkillCredits += (int)xpTable.CharacterLevelSkillCreditList[Level ?? 0];
                    creditEarned = true;
                }

                // break if we reach max
                if (Level == maxLevel)
                {
                    PlayParticleEffect(PlayScript.WeddingBliss, Guid);
                    break;
                }
            }

            if (Level > startingLevel)
            {
                var message = (Level == maxLevel) ? $"You have reached the maximum level of {Level}!" : $"You are now level {Level}!";

                message += (AvailableSkillCredits > 0) ? $"\nYou have {AvailableExperience:#,###0} experience points and {AvailableSkillCredits} skill credits available to raise skills and attributes." : $"\nYou have {AvailableExperience:#,###0} experience points available to raise skills and attributes.";

                var levelUp = new GameMessagePrivateUpdatePropertyInt(this, PropertyInt.Level, Level ?? 1);
                var currentCredits = new GameMessagePrivateUpdatePropertyInt(this, PropertyInt.AvailableSkillCredits, AvailableSkillCredits ?? 0);

                if (Level != maxLevel && !creditEarned)
                {
                    var nextLevelWithCredits = 0;

                    for (int i = (Level ?? 0) + 1; i <= maxLevel; i++)
                    {
                        if (xpTable.CharacterLevelSkillCreditList[i] > 0)
                        {
                            nextLevelWithCredits = i;
                            break;
                        }
                    }
                    message += $"\nYou will earn another skill credit at level {nextLevelWithCredits}.";
                }

                if (Fellowship != null)
                    Fellowship.OnFellowLevelUp(this);

                if (AllegianceNode != null)
                    AllegianceNode.OnLevelUp();

                Session.Network.EnqueueSend(levelUp);

                SetMaxVitals();

                // play level up effect
                PlayParticleEffect(PlayScript.LevelUp, Guid);

                Session.Network.EnqueueSend(new GameMessageSystemChat(message, ChatMessageType.Advancement), currentCredits);

                // Let's take the opportinity to send an activity recommendation to the player.
                var recommendationChain = new ActionChain();
                recommendationChain.AddDelaySeconds(5.0f);
                recommendationChain.AddAction(this, () =>
                {
                    PlayerCommands.HandleSingleRecommendation(Session);
                });
                recommendationChain.EnqueueChain();
            }
        }

        /// <summary>
        /// Spends the amount of XP specified, deducting it from available experience
        /// </summary>
        public bool SpendXP(long amount, bool sendNetworkUpdate = true)
        {
            if (amount > AvailableExperience)
                return false;

            AvailableExperience -= amount;

            if (sendNetworkUpdate)
                Session.Network.EnqueueSend(new GameMessagePrivateUpdatePropertyInt64(this, PropertyInt64.AvailableExperience, AvailableExperience ?? 0));

            return true;
        }

        /// <summary>
        /// Tries to spend all of the players Xp into Attributes, Vitals and Skills
        /// </summary>
        public void SpendAllXp(bool sendNetworkUpdate = true)
        {
            SpendAllAvailableAttributeXp(Strength, sendNetworkUpdate);
            SpendAllAvailableAttributeXp(Endurance, sendNetworkUpdate);
            SpendAllAvailableAttributeXp(Coordination, sendNetworkUpdate);
            SpendAllAvailableAttributeXp(Quickness, sendNetworkUpdate);
            SpendAllAvailableAttributeXp(Focus, sendNetworkUpdate);
            SpendAllAvailableAttributeXp(Self, sendNetworkUpdate);

            SpendAllAvailableVitalXp(Health, sendNetworkUpdate);
            SpendAllAvailableVitalXp(Stamina, sendNetworkUpdate);
            SpendAllAvailableVitalXp(Mana, sendNetworkUpdate);

            foreach (var skill in Skills)
            {
                if (skill.Value.AdvancementClass >= SkillAdvancementClass.Trained)
                    SpendAllAvailableSkillXp(skill.Value, sendNetworkUpdate);
            }
        }

        /// <summary>
        /// Gives available XP of the amount specified, without increasing total XP
        /// </summary>
        public void RefundXP(long amount)
        {
            AvailableExperience += amount;

            var xpUpdate = new GameMessagePrivateUpdatePropertyInt64(this, PropertyInt64.AvailableExperience, AvailableExperience ?? 0);
            Session.Network.EnqueueSend(xpUpdate);
        }

        public void HandleMissingXp()
        {
            var verifyXp = GetProperty(PropertyInt64.VerifyXp) ?? 0;
            if (verifyXp == 0) return;

            var actionChain = new ActionChain();
            actionChain.AddDelaySeconds(5.0f);
            actionChain.AddAction(this, () =>
            {
                var xpType = verifyXp > 0 ? "unassigned experience" : "experience points";

                var msg = $"This character was missing some {xpType} --\nYou have gained an additional {Math.Abs(verifyXp).ToString("N0")} {xpType}!";

                Session.Network.EnqueueSend(new GameMessageSystemChat(msg, ChatMessageType.Broadcast));

                if (verifyXp < 0)
                {
                    // add to character's total XP
                    TotalExperience -= verifyXp;

                    CheckForLevelup();
                }

                RemoveProperty(PropertyInt64.VerifyXp);
            });

            actionChain.EnqueueChain();
        }

        /// <summary>
        /// Returns the total amount of XP required to go from vitae to vitae + 0.01
        /// </summary>
        /// <param name="vitae">The current player life force, ie. 0.95f vitae = 5% penalty</param>
        /// <param name="level">The player DeathLevel, their level on last death</param>
        private double VitaeCPPoolThreshold(float vitae, int level)
        {
            if (Common.ConfigManager.Config.Server.WorldRuleset == Common.Ruleset.EoR)
                return (Math.Pow(level, 2.5) * 2.5 + 20.0) * Math.Pow(vitae, 5.0) + 0.5;
            else
            {
                // http://acpedia.org/wiki/Announcements_-_2005/07_-_Throne_of_Destiny_(expansion)#FAQ_-_AC:TD_Level_Cap_Update
                // "The vitae system has not changed substantially since Asheron's Call launched in 1999.
                // Since that time, the experience awarded by killing creatures has increased considerably.
                // This means that a 5% vitae loss currently is much easier to work off now than it was in the past.
                // In addition, the maximum cost to work off a point of vitae was capped at 12,500 experience points."
                return Math.Min((Math.Pow(level, 2) * 5 + 20) * Math.Pow(vitae, 5.0) + 0.5, 12500);
            }
        }

        /// <summary>
        /// Raise the available XP by a percentage of the current level XP or a maximum
        /// </summary>
        public void GrantLevelProportionalXp(double percent, long min, long max)
        {
            var nextLevelXP = GetXPBetweenLevels(Level.Value, Level.Value + 1);

            var scaledXP = (long)Math.Round(nextLevelXP * percent);

            if (max > 0)
                scaledXP = Math.Min(scaledXP, max);

            if (min > 0)
                scaledXP = Math.Max(scaledXP, min);

            // apply xp modifiers?
            EarnXP(scaledXP, XpType.Quest, Level, null, ShareType.Allegiance);
        }

        /// <summary>
        /// The player earns XP for items that can be leveled up
        /// by killing creatures and completing quests,
        /// while those items are equipped.
        /// </summary>
        public void GrantItemXP(long amount)
        {
            foreach (var item in EquippedObjects.Values.Where(i => i.HasItemLevel))
                GrantItemXP(item, amount);
        }

        public void GrantItemXP(WorldObject item, long amount)
        {
            var prevItemLevel = item.ItemLevel.Value;
            var addItemXP = item.AddItemXP(amount);

            if (addItemXP > 0)
                Session.Network.EnqueueSend(new GameMessagePrivateUpdatePropertyInt64(item, PropertyInt64.ItemTotalXp, item.ItemTotalXp.Value));

            // handle item leveling up
            var newItemLevel = item.ItemLevel.Value;
            if (newItemLevel > prevItemLevel)
            {
                OnItemLevelUp(item, prevItemLevel);

                var actionChain = new ActionChain();
                actionChain.AddAction(this, () =>
                {
                    var msg = $"Your {item.Name} has increased in power to level {newItemLevel}!";
                    Session.Network.EnqueueSend(new GameMessageSystemChat(msg, ChatMessageType.Broadcast));

                    EnqueueBroadcast(new GameMessageScript(Guid, PlayScript.AetheriaLevelUp));
                });
                actionChain.EnqueueChain();
            }
        }

        /// <summary>
        /// Returns the multiplier to XP and Luminance from Trinkets and Augmentations
        /// </summary>
        public float GetXPAndLuminanceModifier(XpType xpType)
        {
            var enchantmentBonus = EnchantmentManager.GetXPBonus();

            var augBonus = 0.0f;
            if (xpType == XpType.Kill && AugmentationBonusXp > 0)
                augBonus = AugmentationBonusXp * 0.05f;

            var modifier = 1.0f + enchantmentBonus + augBonus;
            //Console.WriteLine($"XPAndLuminanceModifier: {modifier}");

            return modifier;
        }
    }
}
