import React, { useState, useRef, useEffect } from 'react';

interface TerminalInputProps {
  prompt: string;
  onSubmit: (value: string) => void;
  autoFocus?: boolean;
}

export function TerminalInput({ prompt, onSubmit, autoFocus = true }: TerminalInputProps) {
  const [value, setValue] = useState('');
  const inputRef = useRef<HTMLInputElement>(null);

  useEffect(() => {
    if (autoFocus && inputRef.current) {
      inputRef.current.focus();
    }
  }, [autoFocus]);

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (value.trim()) {
      onSubmit(value);
    }
  };

  return (
    <form onSubmit={handleSubmit} className="my-4">
      <div className="flex items-center gap-2">
        <span className="opacity-70">{prompt}</span>
        <input
          ref={inputRef}
          type="text"
          value={value}
          onChange={(e) => setValue(e.target.value)}
          className="flex-1 bg-transparent border-b border-[#33ff33] outline-none px-2 py-1"
          style={{
            color: '#33ff33',
            textShadow: '0 0 5px rgba(51, 255, 51, 0.5)',
            caretColor: '#33ff33',
          }}
        />
      </div>
    </form>
  );
}
