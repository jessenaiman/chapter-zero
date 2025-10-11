import React from 'react';

interface TerminalScreenProps {
  children: React.ReactNode;
}

export function TerminalScreen({ children }: TerminalScreenProps) {
  return (
    <div className="relative size-full overflow-hidden bg-black flex items-center justify-center">
      {/* CRT Curvature container */}
      <div className="terminal-crt relative w-full h-full max-w-7xl max-h-[90vh] flex items-center justify-center">
        {/* Scanlines overlay */}
        <div className="absolute inset-0 pointer-events-none scanlines z-10" />
        
        {/* Phosphor glow effect */}
        <div className="absolute inset-0 pointer-events-none phosphor-glow z-10" />
        
        {/* Terminal content */}
        <div className="relative z-20 w-full h-full p-8 md:p-12 overflow-y-auto overflow-x-hidden terminal-content">
          <div className="space-y-4 min-h-full">
            {children}
          </div>
        </div>
        
        {/* Vignette effect */}
        <div className="absolute inset-0 pointer-events-none vignette z-30" />
      </div>
      
      <style jsx>{`
        .terminal-crt {
          background: #000;
          box-shadow: 
            inset 0 0 100px rgba(51, 255, 51, 0.05),
            0 0 80px rgba(51, 255, 51, 0.1);
          border-radius: 8px;
        }
        
        .scanlines {
          background: linear-gradient(
            to bottom,
            transparent 50%,
            rgba(0, 0, 0, 0.3) 50%
          );
          background-size: 100% 4px;
          animation: scanline-move 8s linear infinite;
        }
        
        @keyframes scanline-move {
          0% { transform: translateY(0); }
          100% { transform: translateY(4px); }
        }
        
        .phosphor-glow {
          background: radial-gradient(
            ellipse at center,
            transparent 0%,
            rgba(51, 255, 51, 0.02) 100%
          );
          animation: phosphor-flicker 0.15s infinite alternate;
        }
        
        @keyframes phosphor-flicker {
          0% { opacity: 0.97; }
          100% { opacity: 1; }
        }
        
        .vignette {
          background: radial-gradient(
            ellipse at center,
            transparent 0%,
            rgba(0, 0, 0, 0.6) 100%
          );
        }
        
        .terminal-content {
          font-family: 'Courier New', monospace;
          color: #33ff33;
          text-shadow: 
            0 0 5px rgba(51, 255, 51, 0.5),
            0 0 10px rgba(51, 255, 51, 0.3);
        }
        
        .terminal-content::-webkit-scrollbar {
          width: 8px;
        }
        
        .terminal-content::-webkit-scrollbar-track {
          background: #001100;
        }
        
        .terminal-content::-webkit-scrollbar-thumb {
          background: #33ff33;
          box-shadow: 0 0 5px rgba(51, 255, 51, 0.5);
        }
      `}</style>
    </div>
  );
}
