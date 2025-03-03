using System;
using System.Collections.Generic;
using System.Linq;

using ACE.DatLoader;
using ACE.DatLoader.FileTypes;
using ACE.Database.Models.Shard;
using ACE.Entity;
using ACE.Entity.Enum;
using ACE.Server.Managers;
using ACE.Server.Network;
using ACE.Server.Network.GameEvent.Events;
using ACE.Server.Network.Structure;

namespace ACE.Server.WorldObjects
{
    partial class Player
    {
        // =====================================
        // Character Options
        // =====================================

        public bool GetCharacterOption(CharacterOption option)
        {
            var option1 = option.GetCharacterOptions1Attribute();

            if (option1 != null)
                return GetCharacterOptions1(option1.Option);

            return GetCharacterOptions2(option.GetCharacterOptions2Attribute().Option);
        }

        private bool GetCharacterOptions1(CharacterOptions1 option)
        {
            return (Character.CharacterOptions1 & (int)option) != 0;
        }

        private bool GetCharacterOptions2(CharacterOptions2 option)
        {
            return (Character.CharacterOptions2 & (int)option) != 0;
        }

        public void SetCharacterOption(CharacterOption option, bool value)
        {
            var option1 = option.GetCharacterOptions1Attribute();

            if (option1 != null)
                SetCharacterOptions1(option1.Option, value);
            else
                SetCharacterOptions2(option.GetCharacterOptions2Attribute().Option, value);
        }

        private void SetCharacterOptions1(CharacterOptions1 option, bool value)
        {
            var options = Character.CharacterOptions1;

            if (value)
                options |= (int)option;
            else
                options &= ~(int)option;

            SetCharacterOptions1(options);
        }

        private void SetCharacterOptions2(CharacterOptions2 option, bool value)
        {
            var options = Character.CharacterOptions2;

            if (value)
                options |= (int)option;
            else
                options &= ~(int)option;

            SetCharacterOptions2(options);
        }

        public void SetCharacterOptions1(int value)
        {
            CharacterDatabaseLock.EnterWriteLock();
            try
            {
                Character.CharacterOptions1 = value;
                CharacterChangesDetected = true;
            }
            finally
            {
                CharacterDatabaseLock.ExitWriteLock();
            }
        }

        public void SetCharacterOptions2(int value)
        {
            CharacterDatabaseLock.EnterWriteLock();
            try
            {
                Character.CharacterOptions2 = value;
                CharacterChangesDetected = true;
            }
            finally
            {
                CharacterDatabaseLock.ExitWriteLock();
            }
        }

        public void SetCharacterGameplayOptions(byte[] value)
        {
            CharacterDatabaseLock.EnterWriteLock();
            try
            {
                Character.GameplayOptions = value;
                CharacterChangesDetected = true;
            }
            finally
            {
                CharacterDatabaseLock.ExitWriteLock();
            }
        }

        public bool ToggleTauntSetting()
        {
            if (Common.ConfigManager.Config.Server.WorldRuleset == Common.Ruleset.CustomDM)
            {
                bool newSetting = !GetCharacterOptions2(CharacterOptions2.NotUsed1);
                SetCharacterOptions2(CharacterOptions2.NotUsed1, newSetting);

                CachedAttemptToTaunt = newSetting;
                return newSetting;
            }

            CachedAttemptToTaunt = false;
            return false;
        }

        // =====================================
        // CharacterPropertiesContract
        // =====================================


        // =====================================
        // CharacterPropertiesFillCompBook
        // =====================================


        // =====================================
        // Friends
        // =====================================

        /// <summary>
        /// Adds a friend and updates the database.
        /// </summary>
        /// <param name="friendName">The name of the friend that is being added.</param>
        public void HandleActionAddFriend(string friendName)
        {
            if (string.Equals(friendName, Name, StringComparison.CurrentCultureIgnoreCase))
            {
                ChatPacket.SendServerMessage(Session, "Sorry, but you can't be friends with yourself.", ChatMessageType.Broadcast);
                return;
            }

            // get friend player info
            var friend = PlayerManager.FindByName(friendName);

            if (friend == null)
            {
                ChatPacket.SendServerMessage(Session, "That character does not exist", ChatMessageType.Broadcast);
                return;
            }

            var newFriend = Character.AddFriend(friend.Guid.Full, CharacterDatabaseLock, out var friendAlreadyExists);

            if (friendAlreadyExists)
            {
                ChatPacket.SendServerMessage(Session, "That character is already in your friends list", ChatMessageType.Broadcast);
                return;
            }

            CharacterChangesDetected = true;

            // send network message
            Session.Network.EnqueueSend(new GameEventFriendsListUpdate(Session, GameEventFriendsListUpdate.FriendsUpdateTypeFlag.FriendAdded, newFriend));

            ChatPacket.SendServerMessage(Session, $"{friend.Name} has been added to your friends list.", ChatMessageType.Broadcast);
        }

        /// <summary>
        /// Remove a single friend and update the database.
        /// </summary>
        /// <param name="friendGuid">The ObjectGuid of the friend that is being removed</param>
        public void HandleActionRemoveFriend(uint friendGuid)
        {
            if (!Character.TryRemoveFriend(friendGuid, out var friendToRemove, CharacterDatabaseLock))
            {
                ChatPacket.SendServerMessage(Session, "That character is not in your friends list!", ChatMessageType.Broadcast);
                return;
            }

            CharacterChangesDetected = true;

            // send network message
            Session.Network.EnqueueSend(new GameEventFriendsListUpdate(Session, GameEventFriendsListUpdate.FriendsUpdateTypeFlag.FriendRemoved, friendToRemove));

            // get friend player info
            var friend = PlayerManager.FindByGuid(friendToRemove.FriendId);

            if (friend == null) // shouldn't happen
                ChatPacket.SendServerMessage(Session, "Friend has been removed from your friends list.", ChatMessageType.Broadcast);
            else
                ChatPacket.SendServerMessage(Session, $"{friend.Name} has been removed from your friends list.", ChatMessageType.Broadcast);
        }

        /// <summary>
        /// Delete all friends and update the database.
        /// </summary>
        public void HandleActionRemoveAllFriends()
        {
            // Remove all from DB
            if (Character.ClearAllFriends(CharacterDatabaseLock))
            {
                //ChatPacket.SendServerMessage(Session, "Your friends list has been cleared.", ChatMessageType.Broadcast);
                CharacterChangesDetected = true;
            }
        }

        public bool GetAppearOffline()
        {
            return GetCharacterOption(CharacterOption.AppearOffline);
        }

        /// <summary>
        /// Set the AppearOffline option to the provided value.  It will also send out an update to all online clients that have this player as a friend. This option does not save to the database.
        /// </summary>
        public void SetAppearOffline(bool appearOffline)
        {
            SetCharacterOption(CharacterOption.AppearOffline, appearOffline);
            SendFriendStatusUpdates();
        }


        // =====================================
        // CharacterPropertiesQuestRegistry
        // =====================================


        // =====================================
        // CharacterPropertiesShortcutBar
        // =====================================

        public List<Shortcut> GetShortcuts()
        {
            var shortcuts = new List<Shortcut>();

            foreach (var shortcut in Character.GetShortcuts(CharacterDatabaseLock))
                shortcuts.Add(new Shortcut(shortcut));

            return shortcuts;
        }

        /// <summary>
        /// Handles the adding of items to 1-9 shortcut bar in lower-right corner.<para />
        /// Note that there are two rows. The top row is 1-9, the bottom row has no hotkeys.
        /// </summary>
        public void HandleActionAddShortcut(Shortcut shortcut)
        {
            // When a shortcut is added on top of an existing item, the client automatically sends the RemoveShortcut command for that existing item first, then will add the new item, and re-add the existing item to the appropriate place.

            Character.AddOrUpdateShortcut(shortcut.Index, shortcut.ObjectId, CharacterDatabaseLock);
            CharacterChangesDetected = true;
        }

        /// <summary>
        /// Handles the removing of items from 1-9 shortcut bar in lower-right corner
        /// </summary>
        public void HandleActionRemoveShortcut(uint index)
        {
            if (Character.TryRemoveShortcut(index, out _, CharacterDatabaseLock))
                CharacterChangesDetected = true;
        }


        // =====================================
        // Spell Bar
        // =====================================

        /// <summary>
        /// Will return the spells in the bar, sorted by their position
        /// </summary>
        public List<SpellBarPositions> GetSpellsInSpellBar(int barId)
        {
            var spells = new List<SpellBarPositions>();

            var results = Character.GetSpellsInBar(barId, CharacterDatabaseLock);

            foreach (var result in results)
            {
                var entity = new SpellBarPositions(result.SpellBarNumber, result.SpellBarIndex, result.SpellId);

                spells.Add(entity);
            }

            //spells.Sort((a, b) => a.SpellBarPositionId.CompareTo(b.SpellBarPositionId));

            return spells;
        }

        /// <summary>
        /// This method implements player spell bar management for - adding a spell to a specific spell bar (0 based) at a specific slot (0 based).
        /// </summary>
        public void HandleActionAddSpellFavorite(uint spellId, uint spellBarPositionId, uint spellBarId)
        {
            Character.AddSpellToBar(spellBarId, spellBarPositionId, spellId, CharacterDatabaseLock);

            CharacterChangesDetected = true;
        }

        /// <summary>
        /// This method implements player spell bar management for - removing a spell to a specific spell bar (0 based)
        /// </summary>
        public void HandleActionRemoveSpellFavorite(uint spellId, uint spellBarId)
        {
            if (Character.TryRemoveSpellFromBar(spellBarId, spellId, out _, CharacterDatabaseLock))
                CharacterChangesDetected = true;
        }


        // =====================================
        // CharacterPropertiesTitleBook
        // =====================================

        /// <summary>
        /// Add Title to Title Registry
        /// </summary>
        /// <param name="titleId">Id of Title to Add</param>
        /// <param name="setAsDisplayTitle">If this is true, make this the player's current title</param>
        public void AddTitle(uint titleId, bool setAsDisplayTitle = false)
        {
            if (!Enum.IsDefined(typeof(CharacterTitle), titleId))
                return;

            Character.AddTitleToRegistry(titleId, CharacterDatabaseLock, out var titleAlreadyExists, out var numCharacterTitles);

            bool sendMsg = false;
            bool notifyNewTitle = false;

            if (!titleAlreadyExists)
            {
                CharacterChangesDetected = true;

                NumCharacterTitles = numCharacterTitles;

                sendMsg = true;
                notifyNewTitle = true;

            }

            if (setAsDisplayTitle && CharacterTitleId != titleId)
            {
                CharacterTitleId = (int)titleId;
                sendMsg = true;
            }

            if (sendMsg && FirstEnterWorldDone)
            {
                Session.Network.EnqueueSend(new GameEventUpdateTitle(Session, titleId, setAsDisplayTitle));

                if (notifyNewTitle)
                    Session.Network.EnqueueSend(new GameEventCommunicationTransientString(Session, "You have been granted a new title."));
            }
        }

        public void AddTitle(CharacterTitle title, bool setAsDisplayTitle = false)
        {
            AddTitle((uint)title, setAsDisplayTitle);
        }

        public void HandleActionSetTitle(uint title)
        {
            AddTitle(title, true);
        }

        public void SetTitle(CharacterTitle title)
        {
            AddTitle(title, true);
        }

        public uint EnumMapper_CharacterTitle_FileID = 0x22000041;

        public string GetTitle(CharacterTitle title)
        {
            var titleEnums = DatManager.PortalDat.ReadFromDat<EnumMapper>(EnumMapper_CharacterTitle_FileID);
            if (!titleEnums.IdToStringMap.TryGetValue((uint)title, out var titleEnum))
                return null;

            var hash = SpellTable.ComputeHash(titleEnum);

            var entry = DatManager.LanguageDat.CharacterTitles.StringTableData.FirstOrDefault(i => i.Id == hash);
            if (entry == null)
                return null;

            return entry.Strings.FirstOrDefault();
        }
    }
}
