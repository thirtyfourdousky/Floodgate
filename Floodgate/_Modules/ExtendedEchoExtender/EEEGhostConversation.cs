using FloodgatePatcher;
using HUD;
using RWCustom;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Floodgate._Modules.ExtendedEchoExtender;

internal class EEEGhostConversation : GhostConversation
{
    public EEEGhostConversation(ID id, Ghost ghost, DialogBox dialogBox) : base(id, ghost, dialogBox)
    {

    }
    public override void AddEvents()
    {
        InGameTranslator.LanguageID currentLang = Custom.rainWorld.inGameTranslator.currentLanguage;
        StoryGameSession session = this.ghost.room.game.session as StoryGameSession;
        if (session is not null && ghost is EEEGhost eeeghost && eeeghost.settings.SpecificConv)
        {
            try
            {
                string specificConv;
                specificConv = ResolveConversation(currentLang, session.saveStateNumber.value, session.saveState.deathPersistentSaveData.theMark);
                if(specificConv == null && currentLang != InGameTranslator.LanguageID.English)
                {
                    specificConv = ResolveConversation(InGameTranslator.LanguageID.English, session.saveStateNumber.value, session.saveState.deathPersistentSaveData.theMark);
                }
                if(specificConv != null)
                {
                    string[] specProcessedLines = FGTools.ProcessTimelineConditions(specificConv.Split(new string[] { "\n", "\r\n", "\r" }, StringSplitOptions.RemoveEmptyEntries), ghost.room.game.TimelinePoint);
                    foreach (string line in specProcessedLines)
                    {
                        events.Add(new TextEvent(this, 0, line, 0));
                    }
                    CustomLog.Log("[Extended Echo] Conditional echo conversation file found for " + ghost.worldGhost.ghostID.value);
                    return;
                }
            }
            catch (Exception ex)
            {
                CustomLog.Log("[Extended Echo] Conditional echo conversation file failed for " + ghost.worldGhost.ghostID.value + ", proceeding with default conv (if any)\n"+ ex.ToString());
            }
        }
        //this.events.Add(new Conversation.TextEvent(this, 0, ". . . . .", 0));
        if (!Registry._echoConversations.TryGetValue(id, out _))
        {
            CustomLog.Log("[Extended Echo] Could not find echo conversation for " + ghost.worldGhost.ghostID.value + "!");
            events.Add(new TextEvent(this, 0, "EXTENDED ECHO ERROR: could not find echo conversation!",0));
            return;
        }
        string echoconv = ResolveConversation(currentLang);
        if(echoconv == null && currentLang != InGameTranslator.LanguageID.English)
        {
            echoconv = ResolveConversation(InGameTranslator.LanguageID.English);
        }
        if(echoconv == null)
        {
            CustomLog.Log("[Extended Echo] Could not resolve echo conversation from file");
            events.Add(new TextEvent(this, 0, "EXTENDED ECHO ERROR: could not resolve echo conversation file!", 0));
            return;
        }
        string[] processedLines = FGTools.ProcessTimelineConditions(echoconv.Split(new string[] { "\n", "\r\n", "\r" }, StringSplitOptions.RemoveEmptyEntries), ghost.room.game.TimelinePoint);
        foreach(string line in processedLines)
        {
            events.Add(new TextEvent(this, 0, line, 0));
        }
    }

    /*public delegate void orig_CustomConditions(EEEGhostConversation self);

    public event orig_CustomConditions OnCustomConditions;
    internal void _CustomConditions()
    {
        OnCustomConditions?.Invoke(this);
    }*/

    protected string ResolveConversation(InGameTranslator.LanguageID lang)
    {
        string langshort = LocalizationTranslator.LangShort(lang);
        string path = AssetManager.ResolveFilePath(("text" + Path.DirectorySeparatorChar + "text_" + langshort + Path.DirectorySeparatorChar + "eeechoConv_" + ghost.worldGhost.ghostID + ".txt"));
        bool isdef = lang == InGameTranslator.LanguageID.English;
        if (!System.IO.File.Exists(path))
        {
            if (!isdef)
            {
                return null;
            }
            path = AssetManager.ResolveFilePath(("world" + Path.DirectorySeparatorChar + ghost.worldGhost.world.name + Path.DirectorySeparatorChar + "eeechoConv_" + ghost.worldGhost.ghostID + ".txt"));
            if (!System.IO.File.Exists(path))
            {
                return null;
            }
        }
        if (isdef)
        {
            return Registry.ManageXOREncryption(path);
        }
        return File.ReadAllText(path);
    }

    protected string ResolveConversation(InGameTranslator.LanguageID lang, string slugname, bool hasmark)
    {
        string langshort = LocalizationTranslator.LangShort(lang);

        string path = AssetManager.ResolveFilePath(("text" + Path.DirectorySeparatorChar + "text_" + langshort + Path.DirectorySeparatorChar + "eeechoConv_" + ghost.worldGhost.ghostID + "-" + slugname + "-" + (hasmark ? "mark" : "nomark") + ".txt"));
        if(!System.IO.File.Exists(path))
        {
            path = AssetManager.ResolveFilePath(("text" + Path.DirectorySeparatorChar + "text_" + langshort + Path.DirectorySeparatorChar + "eeechoConv_" + ghost.worldGhost.ghostID + "-" + slugname + ".txt"));
        }
        bool isdef = lang == InGameTranslator.LanguageID.English;
        if (!System.IO.File.Exists(path))
        {
            if (!isdef)
            {
                return null;
            }
            path = AssetManager.ResolveFilePath(("world" + Path.DirectorySeparatorChar + ghost.worldGhost.world.name + Path.DirectorySeparatorChar + "eeechoConv_" + ghost.worldGhost.ghostID + "-" + slugname + "-" + (hasmark ? "mark" : "nomark") + ".txt"));
            if (!System.IO.File.Exists(path))
            {
                path = AssetManager.ResolveFilePath(("world" + Path.DirectorySeparatorChar + ghost.worldGhost.world.name + Path.DirectorySeparatorChar + "eeechoConv_" + ghost.worldGhost.ghostID + "-" + slugname + ".txt"));
            }
            if (!System.IO.File.Exists(path))
            {
                return null;
            }
        }
        if (isdef)
        {
            return Registry.ManageXOREncryption(path);
        }
        return File.ReadAllText(path);
    }
}
