import './Tiles.css';

const Tiles: React.FC = () => {
  return ( 
    <div className="tiles-container">
      <div className="tile">
        <div className="tile-content">
          <h2>[1 2 3]</h2>
          <h3>Long number memory</h3>
          <p>Remember and recall increasingly longer sequences of digits</p>
        </div>
      </div>
      <div className="tile">
        <div className="tile-content">
          <h2>□ □ □</h2>
          <h3>Chimp test</h3>
          <p>Remember and recall an order of numbers</p>
        </div>
      </div>
      <div className="tile">
        <div className="tile-content">
          <h2>□ □ □</h2>
          <h3>Sequence memory</h3>
          <p>Remember and recall increasingly larger sequence of action showed</p>
        </div>
      </div>
    </div>
  );
};

export default Tiles;
