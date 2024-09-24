import { useState } from "react";
import './Dropdown.css';

export const Dropdown = () => {
  const [selectedValue, setSelectedValue] = useState('');
  const options = [
    { value: 'Long number memory', label: 'Long number memory' },
    { value: 'Chimp test', label: 'Chimp test' },
    { value: 'Sequence memory', label: 'Sequence memory' },
  ];

  return (

      <select
        className="custom-select"
        value={selectedValue}
        onChange={(e) => setSelectedValue(e.target.value)}
      >
        {options.map((option) => (
          <option key={option.value} value={option.value}>
            {option.label}
          </option>
        ))}
      </select>

  );
};
