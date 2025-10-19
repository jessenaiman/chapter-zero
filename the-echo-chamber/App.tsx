import React, { useState, useEffect, useCallback, useMemo } from 'react';
import DungeonView from './components/DungeonView';
import DialogueView from './components/DialogueView';
import ResultView from './components/ResultView';
import { DUNGEONS, INTRO_DIALOGUE, INTERPRETATIONS, DREAMWEAVERS, MONSTER_ICON, REACTION_DIALOGUE } from './constants';
import type { GameState, Position, Scores, PlayerChoice, MapSymbol, DreamweaverId, GameObjectType, Dialogue } from './types';
import { generateDungeon } from './utils/dungeonGenerator';
import {
    initAudio,
    playMoveSound,
    playDoorSound,
    playChestSound,
    playMonsterSound,
    playConfirmSound,
    playCancelSound
} from './utils/soundManager';


const INITIAL_SCORES: Scores = { light: 0, shadow: 0, ambition: 0 };
const INITIAL_CHOICES: PlayerChoice[] = [];

const SYMBOL_TO_TYPE_MAP: Record<MapSymbol, GameObjectType> = {
    '+': 'door',
    'M': 'monster',
    'C': 'chest'
};

function App() {
  const [gameState, setGameState] = useState<GameState>('intro');
  const [currentDungeonIndex, setCurrentDungeonIndex] = useState(0);
  const [scores, setScores] = useState<Scores>(INITIAL_SCORES);
  const [choices, setChoices] = useState<PlayerChoice[]>(INITIAL_CHOICES);
  const [playerPosition, setPlayerPosition] = useState<Position>({ x: 0, y: 0 });
  const [dialogue, setDialogue] = useState<Dialogue[]>([]);
  const [currentMap, setCurrentMap] = useState<string[]>([]);
  const [interactionPos, setInteractionPos] = useState<Position | null>(null);
  const [lastChoice, setLastChoice] = useState<PlayerChoice | null>(null);

  const currentDungeon = useMemo(() => DUNGEONS[currentDungeonIndex], [currentDungeonIndex]);

  const resetGame = useCallback(() => {
    setGameState('intro');
    setCurrentDungeonIndex(0);
    setScores(INITIAL_SCORES);
    setChoices(INITIAL_CHOICES);
    setDialogue(INTRO_DIALOGUE as Dialogue[]);
    setLastChoice(null);
  }, []);

  useEffect(() => {
    if (gameState === 'intro') {
      resetGame();
    }
  }, [gameState, resetGame]);

  useEffect(() => {
    if (currentDungeon && (gameState === 'dungeon' || gameState === 'pre_dungeon_dialogue')) {
        const { map, playerStart } = generateDungeon(currentDungeon.generatorStyle);
        setCurrentMap(map);
        setPlayerPosition(playerStart);
    }
  }, [currentDungeon, gameState]);


  const handleInteraction = useCallback((objectType: GameObjectType, position: Position) => {
    if (!currentDungeon) return;

    switch (objectType) {
        case 'door': playDoorSound(); break;
        case 'chest': playChestSound(); break;
        case 'monster': playMonsterSound(); break;
    }

    const gameObject = currentDungeon.objects[objectType];
    const dungeonOwner = currentDungeon.owner;
    const alignedTo = gameObject.aligned_to;

    const newChoice: PlayerChoice = { dungeon: dungeonOwner, choice: objectType, aligned_to: alignedTo };
    setLastChoice(newChoice);

    setScores(prevScores => {
      const newScores = { ...prevScores };
      if (alignedTo === dungeonOwner) {
        newScores[alignedTo] += 2;
      } else {
        newScores[alignedTo] += 1;
      }
      return newScores;
    });

    setChoices(prevChoices => [...prevChoices, newChoice]);

    setCurrentMap(prevMap => {
        const newMap = [...prevMap];
        const row = newMap[position.y];
        newMap[position.y] = row.substring(0, position.x) + '.' + row.substring(position.x + 1);
        return newMap;
    });

    setDialogue([
        { speaker: 'Interaction', text: gameObject.text },
        { speaker: alignedTo, text: INTERPRETATIONS[alignedTo][objectType] }
    ]);
    setGameState('interaction');
  }, [currentDungeon]);

  const handlePlayerMove = useCallback((dx: number, dy: number) => {
    if (gameState !== 'dungeon' || !currentDungeon) return;

    const newPos = { x: playerPosition.x + dx, y: playerPosition.y + dy };
    const targetTile = currentMap[newPos.y]?.[newPos.x] as MapSymbol | undefined;

    if (targetTile && !['#', '~'].includes(targetTile)) {
      playMoveSound();
      if (['+', 'M', 'C'].includes(targetTile)) {
          const objectType = SYMBOL_TO_TYPE_MAP[targetTile];
          if(objectType === 'monster'){
            setGameState('monster_confirm');
            setInteractionPos(newPos);
          } else {
            setPlayerPosition(newPos);
            handleInteraction(objectType, newPos);
          }
      } else {
        setPlayerPosition(newPos);
      }
    }
  }, [gameState, playerPosition, currentMap, handleInteraction, currentDungeon]);

  useEffect(() => {
    const handleKeyDown = (e: KeyboardEvent) => {
      e.preventDefault();
      initAudio(); // Initialize audio on the first user gesture.

      if(gameState === 'dungeon'){
        switch (e.key) {
          case 'ArrowUp': case 'w': handlePlayerMove(0, -1); break;
          case 'ArrowDown': case 's': handlePlayerMove(0, 1); break;
          case 'ArrowLeft': case 'a': handlePlayerMove(-1, 0); break;
          case 'ArrowRight': case 'd': handlePlayerMove(1, 0); break;
        }
      } else if (gameState === 'monster_confirm' && interactionPos) {
        const objectType = 'monster';
        if(e.key.toLowerCase() === 'y'){
            playConfirmSound();
            setPlayerPosition(interactionPos);
            handleInteraction(objectType, interactionPos);
        } else if (e.key.toLowerCase() === 'n') {
            playCancelSound();
            setGameState('dungeon');
            setInteractionPos(null);
        }
      } else if (gameState === 'dungeon_transition') {
          playConfirmSound();
          setCurrentDungeonIndex(prev => prev + 1);
          setGameState('pre_dungeon_dialogue');
      }
    };

    window.addEventListener('keydown', handleKeyDown);
    return () => window.removeEventListener('keydown', handleKeyDown);
  }, [handlePlayerMove, gameState, interactionPos, handleInteraction]);

  const onDialogueFinish = () => {
    if (gameState === 'intro') {
      setGameState('dungeon');
    } else if (gameState === 'interaction') {
      if (currentDungeonIndex < DUNGEONS.length - 1) {
        setGameState('dungeon_transition');
      } else {
        setGameState('outro');
      }
    } else if (gameState === 'pre_dungeon_dialogue') {
        setGameState('dungeon');
    }
  };

  const renderContent = () => {
    switch (gameState) {
      case 'intro':
        return <DialogueView dialogue={dialogue} onFinished={onDialogueFinish} />;

      case 'pre_dungeon_dialogue':
         if (!lastChoice || !currentDungeon) return null;
         const newDialogue: Dialogue[] = (Object.keys(DREAMWEAVERS) as DreamweaverId[]).map(id => ({
             speaker: id,
             text: REACTION_DIALOGUE[id][lastChoice.aligned_to]
         }));
         newDialogue.push({speaker: 'System', text: `Entering ${DREAMWEAVERS[currentDungeon.owner].name}'s Chamber...`});
         return <DialogueView dialogue={newDialogue} onFinished={onDialogueFinish} />;

      case 'dungeon':
        if (!currentDungeon) return null;
        const currentOwner = DREAMWEAVERS[currentDungeon.owner];
        return (
          <div className="flex flex-col items-center gap-4 w-full">
            <h2 className={`text-2xl font-bold ${currentOwner.textColor}`}>
              {currentOwner.name}'s Chamber ({currentDungeonIndex + 1}/3)
            </h2>
            <DungeonView map={currentMap} playerPosition={playerPosition} />
            <p className="text-gray-400">Use Arrow Keys or WASD to move.</p>
          </div>
        );
      case 'interaction':
        return <DialogueView dialogue={dialogue} onFinished={onDialogueFinish} />;
      case 'monster_confirm':
        return (
            <div className="text-center p-6 border-2 border-gray-700 rounded-lg bg-black">
                <p className="text-2xl mb-4">A menacing {MONSTER_ICON} blocks your path.</p>
                <p className="text-xl text-gray-300">Engage?</p>
                <p className="text-lg mt-2">( <span className="text-white font-bold">Y</span>es / <span className="text-white font-bold">N</span>o )</p>
            </div>
        );
      case 'dungeon_transition':
        return (
            <div className="text-center p-6 border-2 border-gray-700 rounded-lg bg-black">
                <p className="text-2xl mb-4">Chamber Complete.</p>
                <p className="text-xl text-gray-300 animate-pulse">Press any key to enter the next chamber...</p>
            </div>
        );
      case 'outro':
        const chosenDreamweaverId = (Object.keys(scores) as DreamweaverId[]).reduce((a, b) => scores[a] >= scores[b] ? a : b);
        return <ResultView chosenDreamweaverId={chosenDreamweaverId} scores={scores} choices={choices} onRestart={resetGame} />;
      default:
        return null;
    }
  };

  return (
    <main className={`flex items-center justify-center min-h-screen bg-black text-white p-4 ${gameState === 'intro' ? 'animate-fade-in' : ''}`}>
      <div className="w-full max-w-7xl">
        {renderContent()}
      </div>
    </main>
  );
}

export default App;
