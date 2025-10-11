import React, { useState, useEffect, useRef } from 'react';

interface AsciiStaticProps {
  onComplete?: () => void;
  duration?: number;
}

export function AsciiStatic({ onComplete, duration = 2000 }: AsciiStaticProps) {
  const [static1, setStatic1] = useState('');
  const [static2, setStatic2] = useState('');
  const onCompleteRef = useRef(onComplete);
  
  const chars = '█▓▒░.:·¯`-_¸,ø¤º°`°º¤ø,¸¸,ø¤º°`';
  const cols = 80;
  const rows = 24;

  // Update ref when onComplete changes
  useEffect(() => {
    onCompleteRef.current = onComplete;
  }, [onComplete]);

  useEffect(() => {
    const interval = setInterval(() => {
      let line1 = '';
      let line2 = '';
      for (let i = 0; i < cols * rows; i++) {
        line1 += chars[Math.floor(Math.random() * chars.length)];
        line2 += chars[Math.floor(Math.random() * chars.length)];
      }
      setStatic1(line1);
      setStatic2(line2);
    }, 50);

    const timeout = setTimeout(() => {
      clearInterval(interval);
      if (onCompleteRef.current) {
        onCompleteRef.current();
      }
    }, duration);

    return () => {
      clearInterval(interval);
      clearTimeout(timeout);
    };
  }, [duration]);

  return (
    <div className="fixed inset-0 z-50 bg-black flex items-center justify-center overflow-hidden">
      <div 
        className="whitespace-pre-wrap break-all opacity-50 blur-[1px]"
        style={{
          color: '#33ff33',
          textShadow: '0 0 10px rgba(51, 255, 51, 0.8)',
          lineHeight: '1',
          fontSize: '8px',
        }}
      >
        {static1}
      </div>
    </div>
  );
}
