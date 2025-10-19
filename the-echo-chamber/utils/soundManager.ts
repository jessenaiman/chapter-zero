// A self-contained sound effect generator using the Web Audio API.

let audioContext: AudioContext | null = null;
let isInitialized = false;

/**
 * Initializes the AudioContext.
 * This must be called as a result of a user gesture (e.g., a click or keypress)
 * to comply with browser autoplay policies.
 */
export const initAudio = () => {
    if (isInitialized || !window.AudioContext) {
        return;
    }
    try {
        audioContext = new (window.AudioContext)();
        isInitialized = true;
    } catch (e) {
        console.error("Web Audio API is not supported in this browser.", e);
    }
};

/**
 * A generic function to play a sound with given parameters.
 * @param type The oscillator type (e.g., 'sine', 'square').
 * @param frequency The frequency of the tone in Hz.
 * @param duration The duration of the sound in seconds.
 * @param volume The volume (gain) from 0.0 to 1.0.
 */
const playTone = (type: OscillatorType, frequency: number, duration: number, volume: number = 0.5) => {
    if (!audioContext) return;

    const oscillator = audioContext.createOscillator();
    const gainNode = audioContext.createGain();

    oscillator.connect(gainNode);
    gainNode.connect(audioContext.destination);

    oscillator.type = type;
    oscillator.frequency.setValueAtTime(frequency, audioContext.currentTime);

    gainNode.gain.setValueAtTime(volume, audioContext.currentTime);
    gainNode.gain.exponentialRampToValueAtTime(0.0001, audioContext.currentTime + duration);

    oscillator.start(audioContext.currentTime);
    oscillator.stop(audioContext.currentTime + duration);
};

// --- Specific Sound Effects ---

export const playMoveSound = () => {
    playTone('square', 80, 0.05, 0.1);
};

export const playDialogueSound = () => {
    playTone('sine', 440, 0.05, 0.1);
};

export const playConfirmSound = () => {
    playTone('sine', 523.25, 0.1, 0.2); // C5
    if (audioContext) {
        setTimeout(() => playTone('sine', 659.25, 0.1, 0.2), 100); // E5
    }
};

export const playCancelSound = () => {
    playTone('sawtooth', 220, 0.15, 0.2);
};

export const playDoorSound = () => {
    if (!audioContext) return;
    const oscillator = audioContext.createOscillator();
    const gainNode = audioContext.createGain();
    oscillator.connect(gainNode);
    gainNode.connect(audioContext.destination);

    oscillator.type = 'sawtooth';
    oscillator.frequency.setValueAtTime(120, audioContext.currentTime);
    oscillator.frequency.exponentialRampToValueAtTime(80, audioContext.currentTime + 0.3);

    gainNode.gain.setValueAtTime(0.3, audioContext.currentTime);
    gainNode.gain.exponentialRampToValueAtTime(0.0001, audioContext.currentTime + 0.3);

    oscillator.start();
    oscillator.stop(audioContext.currentTime + 0.3);
};

export const playChestSound = () => {
    playTone('triangle', 880, 0.1, 0.3); // A5
    if (audioContext) {
        setTimeout(() => playTone('triangle', 1046.50, 0.1, 0.3), 120); // C6
    }
};

export const playMonsterSound = () => {
    if (!audioContext) return;
    const oscillator = audioContext.createOscillator();
    const gainNode = audioContext.createGain();
    oscillator.connect(gainNode);
    gainNode.connect(audioContext.destination);

    oscillator.type = 'square';
    oscillator.frequency.setValueAtTime(100, audioContext.currentTime);
    oscillator.frequency.exponentialRampToValueAtTime(60, audioContext.currentTime + 0.4);

    gainNode.gain.setValueAtTime(0.4, audioContext.currentTime);
    gainNode.gain.linearRampToValueAtTime(0, audioContext.currentTime + 0.4);

    oscillator.start();
    oscillator.stop(audioContext.currentTime + 0.4);
};
