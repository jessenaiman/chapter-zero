import React from 'react';
import { motion } from 'framer-motion';
import type { Position } from '../types';
import { MONSTER_ICON, DOOR_ICON, CHEST_ICON, TILE_SIZE, MAP_WIDTH, MAP_HEIGHT } from '../constants';

interface DungeonViewProps {
  map: string[];
  playerPosition: Position;
}

const VIEWPORT_WIDTH = TILE_SIZE * 30;
const VIEWPORT_HEIGHT = TILE_SIZE * 20;

const DungeonView: React.FC<DungeonViewProps> = ({ map, playerPosition }) => {
  // Helper to check if the player is near a specific position
  const isPlayerNear = (pos: Position, radius: number = 2) => {
    const dx = Math.abs(playerPosition.x - pos.x);
    const dy = Math.abs(playerPosition.y - pos.y);
    return dx <= radius && dy <= radius;
  };

  const mapX = -(playerPosition.x * TILE_SIZE) + (VIEWPORT_WIDTH / 2) - (TILE_SIZE / 2);
  const mapY = -(playerPosition.y * TILE_SIZE) + (VIEWPORT_HEIGHT / 2) - (TILE_SIZE / 2);

  return (
    <div
        className="dungeon-viewport"
        style={{ width: VIEWPORT_WIDTH, height: VIEWPORT_HEIGHT }}
    >
      <motion.div
        className="dungeon-map"
        animate={{ x: mapX, y: mapY }}
        transition={{ type: 'spring', stiffness: 500, damping: 50 }}
        style={{ width: MAP_WIDTH * TILE_SIZE, height: MAP_HEIGHT * TILE_SIZE }}
      >
        {map.map((row, y) =>
          row.split('').map((char, x) => {
            let tileClass = 'tile ';
            let content: string | null = null;
            let objectClass = '';

            switch (char) {
              case '#': tileClass += 'wall'; break;
              case '.': tileClass += 'floor'; break;
              case '~': tileClass += 'water'; break;
              case '+':
                tileClass += 'floor';
                content = DOOR_ICON;
                objectClass = 'text-yellow-400';
                break;
              case 'M':
                tileClass += 'floor';
                content = MONSTER_ICON;
                objectClass = 'text-red-500';
                break;
              case 'C':
                tileClass += 'floor';
                content = CHEST_ICON;
                objectClass = 'text-fuchsia-500';
                break;
              default: tileClass += 'floor'; break;
            }

            const isNear = content && isPlayerNear({ x, y });
            const glowClass = isNear ? 'animate-glow' : '';

            return (
              <div
                key={`${x}-${y}`}
                className={tileClass}
                style={{
                    left: x * TILE_SIZE,
                    top: y * TILE_SIZE,
                    width: TILE_SIZE,
                    height: TILE_SIZE
                }}
              >
                {content && <span className={`${glowClass} ${objectClass}`}>{content}</span>}
              </div>
            );
          })
        )}
      </motion.div>
      <div
        className="player-avatar animate-pulse"
        style={{ width: TILE_SIZE, height: TILE_SIZE }}
      >
        @
      </div>
    </div>
  );
};

export default DungeonView;
