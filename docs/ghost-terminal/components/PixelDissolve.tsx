import React, { useState, useEffect, useRef } from 'react';

interface PixelDissolveProps {
  content: string[];
  onComplete?: () => void;
  duration?: number;
}

export function PixelDissolve({ content, onComplete, duration = 2500 }: PixelDissolveProps) {
  const [displayLines, setDisplayLines] = useState<string[]>(content);
  const [charIndex, setCharIndex] = useState(0);
  const [dissolveProgress, setDissolveProgress] = useState(0);
  const onCompleteRef = useRef(onComplete);
  
  const glitchChars = '█▓▒░.:·¯`-_¸,ø¤º°`°º¤ø,¸¸,ø¤º°`';

  // Update ref when onComplete changes
  useEffect(() => {
    onCompleteRef.current = onComplete;
  }, [onComplete]);

  useEffect(() => {
    // Calculate total characters
    const allText = content.join('');
    const totalChars = allText.length;
    const charDelay = duration / totalChars;
    
    let currentCharIndex = 0;
    
    const interval = setInterval(() => {
      currentCharIndex++;
      setCharIndex(currentCharIndex);
      setDissolveProgress(currentCharIndex / totalChars);

      if (currentCharIndex >= totalChars) {
        clearInterval(interval);
        // Wait a bit before completing
        setTimeout(() => {
          if (onCompleteRef.current) {
            onCompleteRef.current();
          }
        }, 300);
        return;
      }

      // Typewriter-style dissolve: replace characters from left to right
      const newLines: string[] = [];
      let charCounter = 0;
      
      for (const line of content) {
        let newLine = '';
        for (const char of line) {
          if (charCounter < currentCharIndex) {
            // This character should be glitched
            if (char !== ' ' && char !== '\n' && char.trim() !== '') {
              // Multiple levels of glitch based on how long ago it was revealed
              const glitchAge = currentCharIndex - charCounter;
              const glitchIntensity = Math.min(glitchAge / 10, 1);
              
              if (Math.random() < glitchIntensity * 0.7) {
                newLine += glitchChars[Math.floor(Math.random() * glitchChars.length)];
              } else {
                newLine += char;
              }
            } else {
              newLine += char;
            }
          } else {
            // Not yet revealed
            newLine += char;
          }
          charCounter++;
        }
        newLines.push(newLine);
      }

      setDisplayLines(newLines);
    }, charDelay);

    return () => {
      clearInterval(interval);
    };
  }, [content, duration]);

  return (
    <div 
      className="space-y-4"
      style={{
        opacity: 1 - dissolveProgress * 0.8,
        filter: `blur(${dissolveProgress * 3}px)`,
        transition: 'opacity 0.15s, filter 0.15s'
      }}
    >
      {displayLines.map((line, index) => (
        <div 
          key={index} 
          className={line.startsWith('>') ? 'opacity-70' : ''}
          style={{ 
            whiteSpace: 'pre-wrap',
            wordBreak: 'break-word'
          }}
        >
          {line || '\u00A0'}
        </div>
      ))}
    </div>
  );
}
