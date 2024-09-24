import { Dropdown } from '../../components/dropdown/Dropdown';
import './Leaderboard.css';

export const Leaderboard = () => {
    const results = [
        { nr: 1, username: 'Player1', result: '1000', date: '2023-09-20' },
        { nr: 2, username: 'Player2', result: '900', date: '2023-09-19' },
        { nr: 3, username: 'Player3', result: '850', date: '2023-09-18' },
        { nr: 1, username: 'Player1', result: '1000', date: '2023-09-20' },
        { nr: 2, username: 'Player2', result: '900', date: '2023-09-19' },
        { nr: 3, username: 'Player3', result: '850', date: '2023-09-18' },
        { nr: 1, username: 'Player1', result: '1000', date: '2023-09-20' },
        { nr: 2, username: 'Player2', result: '900', date: '2023-09-19' },
        { nr: 3, username: 'Player3', result: '850', date: '2023-09-18' },
        { nr: 1, username: 'Player1', result: '1000', date: '2023-09-20' },
        { nr: 2, username: 'Player2', result: '900', date: '2023-09-19' },
        { nr: 3, username: 'Player3', result: '850', date: '2023-09-18' },
        { nr: 1, username: 'Player1', result: '1000', date: '2023-09-20' },
        { nr: 2, username: 'Player2', result: '900', date: '2023-09-19' },
        { nr: 3, username: 'Player3', result: '850', date: '2023-09-18' },
        { nr: 1, username: 'Player1', result: '1000', date: '2023-09-20' },
        { nr: 2, username: 'Player2', result: '900', date: '2023-09-19' },
        { nr: 3, username: 'Player3', result: '850', date: '2023-09-18' },
        { nr: 1, username: 'Player1', result: '1000', date: '2023-09-20' },
        { nr: 2, username: 'Player2', result: '900', date: '2023-09-19' },
        { nr: 3, username: 'Player3', result: '850', date: '2023-09-18' },
        // Add more results as necessary
      ];
  return (
    <div className='leaderboard-tile'>
        <div className='leaderboard-settings'>
            <p className='choose-game-text'>Choose the game:</p>
            <Dropdown />
        </div>
        <div className='leaderboard'>
        <div className='columns-info'>
          <span>Nr</span>
          <span>Username</span>
          <span>Result</span>
          <span>Date</span>
        </div>
        {/* Render leaderboard results dynamically */}
        {results.map((entry, index) => (
          <div key={index} className='leaderboard-row'>
            <span>{entry.nr}</span>
            <span>{entry.username}</span>
            <span>{entry.result}</span>
            <span>{entry.date}</span>
          </div>
        ))}
      </div>
    </div>
  )
}
