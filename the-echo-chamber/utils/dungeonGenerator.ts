import type { DreamweaverId, Position, MapSymbol } from '../types';
import { MAP_WIDTH, MAP_HEIGHT } from '../constants';

type MapGrid = string[][];

interface Room {
    x: number;
    y: number;
    w: number;
    h: number;
    centerX: number;
    centerY: number;
}

type DungeonLayout = {
  map: string[];
  objectPositions: Record<MapSymbol, Position>;
  playerStart: Position;
};


const createRoom = (grid: MapGrid, room: Room) => {
    for (let y = room.y; y < room.y + room.h; y++) {
        for (let x = room.x; x < room.x + room.w; x++) {
            if (y > 0 && y < MAP_HEIGHT - 1 && x > 0 && x < MAP_WIDTH - 1) {
                grid[y][x] = '.';
            }
        }
    }
};

const createHorizontalTunnel = (grid: MapGrid, x1: number, x2: number, y: number) => {
    for (let x = Math.min(x1, x2); x <= Math.max(x1, x2); x++) {
        if (y > 0 && y < MAP_HEIGHT - 1 && x > 0 && x < MAP_WIDTH - 1) {
            grid[y][x] = '.';
        }
    }
};

const createVerticalTunnel = (grid: MapGrid, y1: number, y2: number, x: number) => {
    for (let y = Math.min(y1, y2); y <= Math.max(y1, y2); y++) {
        if (y > 0 && y < MAP_HEIGHT - 1 && x > 0 && x < MAP_WIDTH - 1) {
            grid[y][x] = '.';
        }
    }
};

const applyStyle = (grid: MapGrid, style: DreamweaverId, rooms: Room[]) => {
    if (rooms.length === 0) return;

    switch(style) {
        case 'light': // Symmetrical pillars in larger rooms
            rooms.filter(r => r.w > 5 && r.h > 5).slice(0, 2).forEach(room => {
                const midX = room.x + Math.floor(room.w / 2);
                const midY = room.y + Math.floor(room.h / 2);
                if(grid[midY - 1]?.[midX] === '.') grid[midY - 1][midX] = '#';
                if(grid[midY + 1]?.[midX] === '.') grid[midY + 1][midX] = '#';
            });
            break;
        case 'shadow': // Puddles of water
            for (let i = 0; i < 25; i++) { // More water
                const room = rooms[Math.floor(Math.random() * rooms.length)];
                const x = room.x + Math.floor(Math.random() * room.w);
                const y = room.y + Math.floor(Math.random() * room.h);
                if (grid[y]?.[x] === '.') {
                    grid[y][x] = '~';
                }
            }
            break;
        case 'ambition': // Partition walls in rooms
             rooms.filter(r => r.w > 6 || r.h > 6).slice(0, 2).forEach(room => {
                 if (room.w > room.h) { // partition vertically
                     const partX = room.x + Math.floor(room.w / 2);
                     for (let y = room.y; y < room.y + room.h; y++) {
                         grid[y][partX] = '#';
                     }
                     grid[room.y + Math.floor(Math.random() * (room.h - 2)) + 1][partX] = '.'; // leave a random gap
                 } else { // partition horizontally
                     const partY = room.y + Math.floor(room.h / 2);
                     for (let x = room.x; x < room.x + room.w; x++) {
                         grid[partY][x] = '#';
                     }
                     grid[partY][room.x + Math.floor(Math.random() * (room.w - 2)) + 1] = '.'; // leave a random gap
                 }
             });
            break;
    }
};

const isReachable = (grid: MapGrid, start: Position, targets: Position[]): boolean => {
    const queue: Position[] = [start];
    const visited = new Set<string>([`${start.x},${start.y}`]);
    const targetSet = new Set<string>(targets.map(t => `${t.x},${t.y}`));

    if (targetSet.has(`${start.x},${start.y}`)) {
        targetSet.delete(`${start.x},${start.y}`);
    }

    while(queue.length > 0){
        if (targetSet.size === 0) return true;
        const curr = queue.shift()!;

        const neighbors = [{dx:0, dy:1}, {dx:0, dy:-1}, {dx:1, dy:0}, {dx:-1, dy:0}];
        for(const {dx, dy} of neighbors){
            const next = {x: curr.x + dx, y: curr.y + dy};
            const key = `${next.x},${next.y}`;

            if(next.x >= 0 && next.x < MAP_WIDTH && next.y >= 0 && next.y < MAP_HEIGHT &&
               grid[next.y][next.x] !== '#' && !visited.has(key)){

                visited.add(key);
                if (targetSet.has(key)) {
                    targetSet.delete(key);
                }
                queue.push(next);
            }
        }
    }
    return targetSet.size === 0;
}

export function generateDungeon(style: DreamweaverId): DungeonLayout {
    let generationAttempts = 0;
    while(generationAttempts < 50) { // Increased attempts for more complex generation
        generationAttempts++;

        const grid: MapGrid = Array.from({ length: MAP_HEIGHT }, () => Array(MAP_WIDTH).fill('#'));
        const rooms: Room[] = [];
        const minRoomSize = 4;
        const maxRoomSize = 9;
        const maxRooms = 15;

        for (let i = 0; i < maxRooms; i++) {
            const w = Math.floor(Math.random() * (maxRoomSize - minRoomSize + 1)) + minRoomSize;
            const h = Math.floor(Math.random() * (maxRoomSize - minRoomSize + 1)) + minRoomSize;
            const x = Math.floor(Math.random() * (MAP_WIDTH - w - 2)) + 1;
            const y = Math.floor(Math.random() * (MAP_HEIGHT - h - 2)) + 1;

            const newRoom: Room = { x, y, w, h, centerX: x + Math.floor(w / 2), centerY: y + Math.floor(h / 2) };

            const overlaps = rooms.some(otherRoom =>
                newRoom.x < otherRoom.x + otherRoom.w + 1 && newRoom.x + newRoom.w + 1 > otherRoom.x &&
                newRoom.y < otherRoom.y + otherRoom.h + 1 && newRoom.y + newRoom.h + 1 > otherRoom.y
            );

            if (!overlaps) {
                createRoom(grid, newRoom);

                if (rooms.length > 0) {
                    const prevRoom = rooms[rooms.length - 1];
                    if (Math.random() > 0.5) {
                        createHorizontalTunnel(grid, prevRoom.centerX, newRoom.centerX, prevRoom.centerY);
                        createVerticalTunnel(grid, prevRoom.centerY, newRoom.centerY, newRoom.centerX);
                    } else {
                        createVerticalTunnel(grid, prevRoom.centerY, newRoom.centerY, prevRoom.centerX);
                        createHorizontalTunnel(grid, prevRoom.centerX, newRoom.centerX, newRoom.centerY);
                    }
                }
                rooms.push(newRoom);
            }
        }

        if (rooms.length < 4) continue;

        applyStyle(grid, style, rooms);

        const floorPositions: Position[] = [];
        for (const room of rooms) {
            for (let y = room.y + 1; y < room.y + room.h - 1; y++) {
                for (let x = room.x + 1; x < room.x + room.w - 1; x++) {
                    if (grid[y][x] === '.') {
                        floorPositions.push({ x, y });
                    }
                }
            }
        }

        if(floorPositions.length < 4) continue;

        for (let i = floorPositions.length - 1; i > 0; i--) {
            const j = Math.floor(Math.random() * (i + 1));
            [floorPositions[i], floorPositions[j]] = [floorPositions[j], floorPositions[i]];
        }

        const playerStart = floorPositions.pop()!;
        const doorPos = floorPositions.pop()!;
        const monsterPos = floorPositions.pop()!;
        const chestPos = floorPositions.pop()!;
        const objectPositions = [doorPos, monsterPos, chestPos];

        if(isReachable(grid, playerStart, objectPositions)) {
            grid[doorPos.y][doorPos.x] = '+';
            grid[monsterPos.y][monsterPos.x] = 'M';
            grid[chestPos.y][chestPos.x] = 'C';

            return {
                map: grid.map(row => row.join('')),
                objectPositions: { '+': doorPos, 'M': monsterPos, 'C': chestPos },
                playerStart,
            };
        }
    }

    console.warn("Dungeon generation failed, using fallback.");
    const fallbackGrid = Array.from({ length: MAP_HEIGHT }, () => Array(MAP_WIDTH).fill('.'));
    for (let y = 0; y < MAP_HEIGHT; y++) {
        for (let x = 0; x < MAP_WIDTH; x++) {
            if (y === 0 || y === MAP_HEIGHT - 1 || x === 0 || x === MAP_WIDTH - 1) {
                fallbackGrid[y][x] = '#';
            }
        }
    }
    const playerStart = {x: 5, y: 5};
    const monsterPos = {x: MAP_WIDTH - 10, y: 10};
    const chestPos = {x: 15, y: MAP_HEIGHT - 5};
    const doorPos = {x: MAP_WIDTH - 15, y: 5};
    fallbackGrid[monsterPos.y][monsterPos.x] = 'M';
    fallbackGrid[chestPos.y][chestPos.x] = 'C';
    fallbackGrid[doorPos.y][doorPos.x] = '+';

    return {
         map: fallbackGrid.map(row => row.join('')),
         objectPositions: { '+': doorPos, 'M': monsterPos, 'C': chestPos },
         playerStart,
    };
}
