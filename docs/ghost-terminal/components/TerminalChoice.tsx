import React from 'react';

interface TerminalChoiceProps {
  choices: string[];
  onSelect: (index: number, choice: string) => void;
  prefix?: string;
}

export function TerminalChoice({ choices, onSelect, prefix = '[ ' }: TerminalChoiceProps) {
  return (
    <div className="space-y-2 my-4">
      {choices.map((choice, index) => (
        <button
          key={index}
          onClick={() => onSelect(index, choice)}
          className="block w-full text-left px-4 py-2 border border-[#33ff33] hover:bg-[#33ff33] hover:text-black transition-all duration-200 group"
          style={{
            textShadow: '0 0 5px rgba(51, 255, 51, 0.5)',
          }}
        >
          <span className="opacity-70 group-hover:opacity-100">{prefix}</span>
          <span className="group-hover:tracking-wide transition-all duration-200">
            {choice}
          </span>
          <span className="opacity-70 group-hover:opacity-100"> ]</span>
        </button>
      ))}
    </div>
  );
}
