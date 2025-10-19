export type DreamweaverId = 'light' | 'shadow' | 'ambition';
export type GameObjectType = 'door' | 'monster' | 'chest';
export type MapSymbol = '+' | 'M' | 'C';

export interface Dreamweaver {
  id: DreamweaverId;
  name: string;
  color: string;
  textColor: string;
}

export interface GameObject {
  type: GameObjectType;
  text: string;
  aligned_to: DreamweaverId;
  symbol: MapSymbol;
}

export interface Dungeon {
  owner: DreamweaverId;
  generatorStyle: DreamweaverId;
  objects: Record<GameObjectType, Omit<GameObject, 'type' | 'symbol'>>;
}

export interface PlayerChoice {
  dungeon: DreamweaverId;
  choice: GameObjectType;
  aligned_to: DreamweaverId;
}

export type Scores = Record<DreamweaverId, number>;

export type GameState = 'intro' | 'pre_dungeon_dialogue' | 'dungeon' | 'interaction' | 'outro' | 'monster_confirm' | 'dungeon_transition';

export interface Position {
  x: number;
  y: number;
}

export type Speaker = DreamweaverId | 'Omega' | 'System' | 'Interaction';

export interface Dialogue {
    speaker: Speaker;
    text: string;
}
