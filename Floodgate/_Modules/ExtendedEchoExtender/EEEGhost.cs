using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Floodgate._Modules.ExtendedEchoExtender;

public class EEEGhost : Ghost
{
    public GhostWorldPresence.GhostID ghostID;
    public Conversation.ID conversationID;
    public string conversation;
    public EchoSettings settings;
    public EEEGhost(Room room, PlacedObject placedObject, GhostWorldPresence worldGhost) : base(room, placedObject, worldGhost)
    {
        this.placedObject = placedObject;
        pos = placedObject.pos;
        this.worldGhost = worldGhost;
        ghostID = worldGhost.ghostID;
        if(!Registry.echoesSettings.TryGetValue(ghostID, out settings))
        {
            settings = new(room.game.StoryCharacter) { Room = room.abstractRoom.name };
        }
        scale = 0.75f * settings.SizeMultiplier;
        lightSpriteScale = 0f;
        this.rags.conRad = 30f * this.scale;
        this.defaultFlip = settings.Flip;
        this.conversationID = Registry.GetConvID(ghostID.value);
        Registry._echoConversations.TryGetValue(conversationID, out conversation);
        InitializeSprites();
        if (room.world.rainCycle.BlizzardWorldActive)
        {
            room.roomSettings.RainIntensity = 0.04f;
        }
    }
    public override void StartConversation()
    {
        if (room.game.cameras[0].hud.dialogBox == null)
        {
            room.game.cameras[0].hud.InitDialogBox();
        }
        currentConversation = new EEEGhostConversation(conversationID, this, room.game.cameras[0].hud.dialogBox);
        conversationActive = true;
        //room.world.worldGhost = room.world.ExtraGhost(ghostID);
    }
}
