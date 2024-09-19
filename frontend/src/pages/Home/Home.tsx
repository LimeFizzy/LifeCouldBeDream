import { useState, useEffect } from 'react';
import {Tile} from '../../components/tile/Tile'; 
import './Home.css';

interface GameDTO {
  gameID: number;
  title: string;
  description: string;
  icon: string;
  altText: string;
  route: string;
}

export const Home = () => {
  const [games, setGames] = useState<GameDTO[]>([]);

  useEffect(() => {
    const fetchGames = async () => {
      try {
        const response = await fetch('http://localhost:5217/games');
        if (!response.ok) {
          throw new Error(`Error: ${response.statusText}`);
        }
        const data: GameDTO[] = await response.json();
        setGames(data);
      } catch (error) {
        console.error('Error fetching games:', error);
      }
    };

    fetchGames();
  }, []);

  return (
    <div className="tiles-container">
      {games.map((game) => (
        <Tile 
          key={game.gameID}
          icon={game.icon}
          altText={game.altText}
          title={game.title}
          description={game.description}
          route={game.route}
        />
      ))}
    </div>
  );
};

