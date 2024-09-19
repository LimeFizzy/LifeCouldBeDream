import { useNavigate } from 'react-router-dom';
import './Tile.css';

interface TileProps {
  icon: string;
  altText: string;
  title: string;
  description: string;
  route: string;
}

export const Tile = ({icon, altText, title, description, route}: TileProps) => {
  const navigate = useNavigate();

  return (
    <div className="tile" onClick={() => navigate(route)}>
      <div className="tile-content">
        <img src={icon} alt={altText} className="center-icon"/>
        <h3>{title}</h3>
        <p>{description}</p>
      </div>
    </div>
  );
};

