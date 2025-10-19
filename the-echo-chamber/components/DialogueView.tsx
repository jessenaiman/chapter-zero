
import React, { useState, useEffect } from 'react';
import { DREAMWEAVERS } from '../constants';
import type { DreamweaverId, Dialogue } from '../types';
import { playDialogueSound } from '../utils/soundManager';

interface DialogueViewProps {
  dialogue: Dialogue[];
  onFinished: () => void;
  speed?: number;
}

const DialogueView: React.FC<DialogueViewProps> = ({ dialogue, onFinished, speed = 40 }) => {
  const [currentLineIndex, setCurrentLineIndex] = useState(0);
  const [currentText, setCurrentText] = useState('');
  const [isFinished, setIsFinished] = useState(false);

  useEffect(() => {
    if (currentLineIndex < dialogue.length) {
      const line = dialogue[currentLineIndex];
      if (currentText.length < line.text.length) {
        const timeout = setTimeout(() => {
          setCurrentText(line.text.slice(0, currentText.length + 1));
          // Play sound for non-space characters
          if (line.text.slice(currentText.length, currentText.length + 1).trim() !== '') {
            playDialogueSound();
          }
        }, speed);
        return () => clearTimeout(timeout);
      } else {
         const lineTimeout = setTimeout(() => {
            setCurrentLineIndex(currentLineIndex + 1);
            setCurrentText('');
         }, 750);
         return () => clearTimeout(lineTimeout);
      }
    } else {
        setIsFinished(true);
        onFinished();
    }
  }, [currentLineIndex, currentText, dialogue, onFinished, speed]);

  const currentLine = dialogue[currentLineIndex];

  const getSpeakerStyle = (speaker: Dialogue['speaker']) => {
    if (speaker === 'Omega' || speaker === 'System') return 'text-gray-400';
    if (speaker === 'Interaction') return 'text-cyan-400';
    return DREAMWEAVERS[speaker]?.textColor || 'text-white';
  };

  const speakerName = (speaker: Dialogue['speaker']) => {
    if(speaker === 'Interaction') return "Narrator";
    return DREAMWEAVERS[speaker as DreamweaverId]?.name || speaker;
  }

  if (!currentLine) return null;

  return (
    <div className="w-full max-w-3xl p-4 border-2 border-gray-700 bg-black bg-opacity-80 rounded-lg min-h-[100px]">
      <p className={`font-bold ${getSpeakerStyle(currentLine.speaker)}`}>
        {speakerName(currentLine.speaker)}:
      </p>
      <p className="text-xl text-gray-200">{currentText}</p>
    </div>
  );
};

export default DialogueView;
