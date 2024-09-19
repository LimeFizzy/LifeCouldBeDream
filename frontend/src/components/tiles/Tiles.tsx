import './Tiles.css';
import chimpIcon from '../../assets/Chimp_Icon.svg';
import sequenceIcon from '../../assets/Sequence_Icon.svg';

const Tiles: React.FC = () => {
  return (
    <div className="tiles-container">
      {/* Number Tile */}
      <div className="tile">
        <div className="tile-content">
          <h2>[1 2 3]</h2>
          <h3>Long number memory</h3>
          <p>Remember and recall increasingly longer sequences of digits</p>
        </div>
      </div>
      
      {/* Chimp Tile */}
      <div className="tile">
        <div className="tile-content">
          <img src={chimpIcon} alt="Center Icon" className="center-icon" /> {}
          <h3>Chimp test</h3>
          <p>Remember and recall an order of numbers</p>
        </div>
      </div>
      
      {/* Sequence Tile */}
      <div className="tile">
        <div className="tile-content">
        <img src={sequenceIcon} alt="Sequence Icon" className="center-icon" /> {}
          <h3>Sequence memory</h3>
          <p>Remember and recall increasingly larger sequence of action showed</p>
        </div>
      </div>
    </div>
  );
};

export default Tiles;
