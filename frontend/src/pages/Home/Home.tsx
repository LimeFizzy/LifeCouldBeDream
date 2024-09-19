import {Tile} from '../../components/tile/Tile'; 
import './Home.css';

export const Home: React.FC = () => {
  return (
    <div className="tiles-container">
      <Tile 
        icon={''} 
        altText={'Long number memory icon'} 
        title={'Long number memory'} 
        description={'Remember and recall increasingly longer sequences of digits'}
      />
      <Tile 
        icon={'src/assets/Chimp_Icon.svg'} 
        altText={'Chimp test icon'} 
        title={'Chimp test'} 
        description={'Remember and recall an order of numbers'}
      />
      <Tile 
        icon={'src/assets/Sequence_Icon.svg'} 
        altText={'Sequence memory icon'} 
        title={'Sequence Icon'} 
        description={'Remember and recall increasingly larger sequence of action showed'}
      />
    </div>
  );
};

