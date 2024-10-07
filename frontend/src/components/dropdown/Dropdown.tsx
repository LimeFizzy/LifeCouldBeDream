import { useCallback, useState } from "react";
import "./Dropdown.css";

interface DropdownProps {
  onSelectChange: (value: string) => void;
}

enum GameTypes {
  LONG_NUMBER = "longNumberMemory",
  SEQUENCE = "sequenceMemory",
  CHIMP = "chimpTest",
}

export const Dropdown = ({ onSelectChange }: DropdownProps) => {
  const [selectedValue, setSelectedValue] = useState("");
  const options = [
    { value: GameTypes.LONG_NUMBER, label: "Long number memory" },
    { value: GameTypes.CHIMP, label: "Chimp test" },
    { value: GameTypes.SEQUENCE, label: "Sequence memory" },
  ];

  const handleChange = useCallback(
    (e: React.ChangeEvent<HTMLSelectElement>) => {
      const value = e.target.value;
      setSelectedValue(value);
      onSelectChange(value);
    },
    []
  );

  return (
    <select
      className="custom-select"
      value={selectedValue}
      onChange={handleChange}
    >
      {options.map((option) => (
        <option key={option.value} value={option.value}>
          {option.label}
        </option>
      ))}
    </select>
  );
};
