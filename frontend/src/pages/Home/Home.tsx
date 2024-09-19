import {Tile} from '../../components/tile/Tile'; 
import './Home.css';

export const Home: React.FC = () => {
  return (
    <div className="tiles-container">
      <Tile 
        icon={'src/assets/longNumberIcon.svg'} 
        altText={'Long number memory icon'} 
        title={'Long number memory'} 
        description={'Remember and recall increasingly longer sequences of digits'}
      />
      <Tile 
        icon={'src/assets/chimpIcon.svg'} 
        altText={'Chimp test icon'} 
        title={'Chimp test'} 
        description={'Remember and recall an order of numbers'}
      />
      <Tile 
        icon={'src/assets/sequenceIcon.svg'} 
        altText={'Sequence memory icon'} 
        title={'Sequence Icon'} 
        description={'Remember and recall increasingly larger sequence of action showed'}
      />
    </div>
  );
};

