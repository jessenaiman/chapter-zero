import React, { useState, useEffect, useRef } from 'react';

interface TypewriterTextProps {
  text: string;
  delay?: number;
  onComplete?: () => void;
  className?: string;
  prefix?: string;
}

export function TypewriterText({ 
  text, 
  delay = 50, 
  onComplete, 
  className = '',
  prefix = ''
}: TypewriterTextProps) {
  const [displayedText, setDisplayedText] = useState('');
  const [currentIndex, setCurrentIndex] = useState(0);
  const completedRef = useRef(false);

  // Reset state when text changes
  useEffect(() => {
    setDisplayedText('');
    setCurrentIndex(0);
    completedRef.current = false;
  }, [text]);

  useEffect(() => {
    if (currentIndex < text.length) {
      const timeout = setTimeout(() => {
        setDisplayedText(prev => prev + text[currentIndex]);
        setCurrentIndex(prev => prev + 1);
      }, delay);

      return () => clearTimeout(timeout);
    } else if (currentIndex === text.length && !completedRef.current) {
      completedRef.current = true;
      if (onComplete) {
        onComplete();
      }
    }
  }, [currentIndex, text.length, delay]);

  return (
    <div className={className}>
      {prefix && <span className="opacity-70">{prefix}</span>}
      {displayedText}
      {currentIndex < text.length && (
        <span className="animate-pulse">█</span>
      )}
    </div>
  );
}
