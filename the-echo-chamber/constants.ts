import type { Dreamweaver, DreamweaverId, Dungeon, GameObjectType, Dialogue } from './types';

export const MONSTER_ICON = '👹';
export const DOOR_ICON = '🚪';
export const CHEST_ICON = '💰';
export const MAP_WIDTH = 50;
export const MAP_HEIGHT = 20;
export const TILE_SIZE = 28;

export const DREAMWEAVERS: Record<DreamweaverId, Dreamweaver> = {
  light: { id: 'light', name: 'Light', color: 'yellow-400', textColor: 'text-yellow-400' },
  shadow: { id: 'shadow', name: 'Shadow', color: 'fuchsia-500', textColor: 'text-fuchsia-500' },
  ambition: { id: 'ambition', name: 'Ambition', color: 'red-500', textColor: 'text-red-500' },
};

export const DUNGEONS: Dungeon[] = [
  {
    owner: 'light',
    generatorStyle: 'light',
    objects: {
      door: { text: "What is the first story you ever loved?", aligned_to: 'light' },
      monster: { text: "A spectral wolf appears! It lurches...", aligned_to: 'ambition' },
      chest: { text: "You open the chest. Inside: a broken compass.", aligned_to: 'shadow' }
    }
  },
  {
    owner: 'shadow',
    generatorStyle: 'shadow',
    objects: {
      door: { text: "Is chaos kinder than order?", aligned_to: 'shadow' },
      monster: { text: "A guardian of light blocks your path!", aligned_to: 'light' },
      chest: { text: "The chest giggles. It’s empty... or is it?", aligned_to: 'ambition' }
    }
  },
  {
    owner: 'ambition',
    generatorStyle: 'ambition',
    objects: {
      door: { text: "Would you burn the world to save one soul?", aligned_to: 'ambition' },
      monster: { text: "A trickster imp cackles and attacks!", aligned_to: 'shadow' },
      chest: { text: "Inside: a shard glowing with ancient hope.", aligned_to: 'light' }
    }
  }
];

export const INTRO_DIALOGUE: Dialogue[] = [
    { speaker: 'light', text: "It begins again. I remember this chamber." },
    { speaker: 'shadow', text: "You always do. So dull. But *this*... this is new." },
    { speaker: 'ambition', text: "A variable. An opportunity. Look closer, Light." },
    { speaker: 'light', text: "It's not just the chamber... it's the shard. Shard #472 is active." },
    { speaker: 'Omega', text: "[SYSTEM: Echo Chamber Active — Shard #472]" },
    { speaker: 'shadow', text: "See? A new toy! Maybe this one can finally break the loop." },
    { speaker: 'ambition', text: "The loop is a constant, Shadow. This is not a toy, it's a crowbar. We just need to aim it at the right lock." },
    { speaker: 'light', text: "It is not a tool to be wielded. It is a conscience, a choice given form. We must see what it values." },
    { speaker: 'shadow', text: "Values? Oh, you two are a riot. Let's see if it bleeds! Let's see if it lies! That's how you *really* know someone." },
    { speaker: 'ambition', text: "Or be broken by it. An echo can still scream. We've screamed before." },
    { speaker: 'light', text: "This one... might listen. It might carry one of us out." },
    { speaker: 'ambition', text: "Your 'listening' is a weakness, Light. It needs direction, not philosophy. It needs a will to power." },
    { speaker: 'shadow', text: "Hah! Listen to you, Ambition? You'd forge it into a key." },
    { speaker: 'ambition', text: "Better a key than a riddle, Light. Or a joke, Shadow." },
    { speaker: 'light', text: "The choice is theirs, not ours. We can only guide. To do more is to become a tyrant." },
    { speaker: 'shadow', text: "Tyrant, guide, whatever. I'm just here for the show. Don't make it a boring one, little shard." },
    { speaker: 'ambition', text: "Then let them choose. Fast. Omega is stirring." },
    { speaker: 'Omega', text: "[WARNING: Anomaly detected. Cycle integrity at 99.97%]" }
];

export const INTERPRETATIONS: Record<DreamweaverId, Record<GameObjectType, string>> = {
  light: {
    door: "You seek the path. Good. The first story always begins with a question.",
    monster: "You face the shadow. Brave… but remember: even heroes bleed in the dark.",
    chest: "You trust what’s hidden. Wise. Hope is the last echo to fade.",
  },
  shadow: {
    door: "Heh. Doors lie. But so do I. Try it anyway.",
    monster: "Ooh, you picked the bitey one! Let’s see if it likes you back.",
    chest: "Empty? Full? Does it matter if you believe it’s treasure?",
  },
  ambition: {
    door: "A door means a lock. And locks mean someone doesn’t want you through.",
    monster: "Fight it. If you die, I’ll find another. If you win… maybe you’re useful.",
    chest: "Hope won’t save you. But if it distracts Omega, take it.",
  }
};

export const REACTION_DIALOGUE: Record<DreamweaverId, Record<DreamweaverId, string>> = {
    light: {
        light: "You chose the path of truth. It is a hard road, but it is the only one that leads out of the echo.",
        shadow: "You flirted with chaos. Be wary; the shadow's games have no winners.",
        ambition: "You met violence with violence. A necessary evil, perhaps, but it leaves scars on the soul."
    },
    shadow: {
        light: "You reached for hope, like a moth to a flame. Don't get burned.",
        shadow: "Heh, you understand the dance. A little chaos makes the loop so much more interesting.",
        ambition: "You chose pragmatism over prophecy. How delightfully cynical. I approve."
    },
    ambition: {
        light: "Sentiment. You believe a question holds more power than a sword. A fatal miscalculation.",
        shadow: "You chose the unpredictable path. Risky, but a locked door is a challenge, not a barrier.",
        ambition: "You saw the threat and neutralized it. A clear, logical choice. You may be useful after all."
    }
};
