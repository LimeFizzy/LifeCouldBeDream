import './Tile.css';

interface TileProps {
  icon: string;
  altText: string;
  title: string;
  description: string;
}

export const Tile = ({icon, altText, title, description}: TileProps) => {
  return (
    <div className="tile">
      <div className="tile-content">
        <img src={icon} alt={altText} className="center-icon" sizes='50px'/>
        <h3>{title}</h3>
        <p>{description}</p>
      </div>
    </div>
  );
};

