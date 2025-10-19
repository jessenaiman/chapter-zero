
import React from 'react';
import type { PlayerChoice, Scores, DreamweaverId } from '../types';
import { DREAMWEAVERS } from '../constants';
import { playConfirmSound } from '../utils/soundManager';

interface ResultViewProps {
  chosenDreamweaverId: DreamweaverId;
  scores: Scores;
  choices: PlayerChoice[];
  onRestart: () => void;
}

const capitalize = (s: string) => s.charAt(0).toUpperCase() + s.slice(1);

const ResultView: React.FC<ResultViewProps> = ({ chosenDreamweaverId, scores, choices, onRestart }) => {
  const chosenDreamweaver = DREAMWEAVERS[chosenDreamweaverId];
  const resultObject = {
    scene: "scene2_result",
    chosen_dreamweaver: chosenDreamweaver.name,
    scores: scores,
    choices: choices.map(c => ({
        chamber: `${DREAMWEAVERS[c.dungeon].name}'s Chamber`,
        choice: c.choice,
        aligned_with: DREAMWEAVERS[c.aligned_to].name
    }))
  };

  const handleRestartClick = () => {
    playConfirmSound();
    onRestart();
  };

  return (
    <div className="flex flex-col items-center justify-center min-h-screen p-4 text-left w-full max-w-3xl mx-auto">
        <h1 className="text-3xl font-bold mb-6">Echo Chamber Analysis Complete</h1>

        <div className={`text-center p-6 border-2 ${chosenDreamweaver.textColor.replace('text-','border-')} rounded-lg mb-8 w-full`}>
            <p className="text-xl">The echo that resonates with you is...</p>
            <p className={`text-4xl font-bold ${chosenDreamweaver.textColor}`}>{chosenDreamweaver.name}</p>
            <p className="mt-4 text-lg">"{chosenDreamweaver.name}: I’m coming with you. Don’t lose me this time."</p>
        </div>

        <div className="w-full bg-gray-900 border-2 border-gray-700 rounded-lg p-6 mb-8">
            <h2 className="text-2xl font-bold mb-4 text-center">Journey Log</h2>
            <ul className="space-y-3">
                {choices.map((choice, index) => {
                    const chamberOwner = DREAMWEAVERS[choice.dungeon];
                    const alignment = DREAMWEAVERS[choice.aligned_to];
                    return (
                        <li key={index} className="text-lg border-b border-gray-700 pb-2 last:border-b-0">
                           <span className={`${chamberOwner.textColor}`}>In {chamberOwner.name}'s Chamber:</span>
                           <span> You chose the {capitalize(choice.choice)}, an act aligned with </span>
                           <span className={`font-bold ${alignment.textColor}`}>{alignment.name}</span>.
                        </li>
                    );
                })}
            </ul>
        </div>

        <div className="w-full bg-gray-900 border-2 border-gray-700 rounded-lg p-6 font-mono text-sm overflow-x-auto mb-8">
            <h2 className="text-lg font-bold mb-2 text-gray-400">// RAW DATA OUTPUT</h2>
            <pre className="text-white whitespace-pre-wrap">{JSON.stringify(resultObject, null, 2)}</pre>
        </div>

        <button
            onClick={handleRestartClick}
            className="px-8 py-3 bg-gray-700 text-white font-bold rounded-lg hover:bg-gray-600 transition-colors duration-300"
        >
            Restart Simulation
        </button>
    </div>
  );
};

export default ResultView;
