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
  const [selectedValue, setSelectedValue] = useState(
    GameTypes.LONG_NUMBER as string
  );
  const [isOpen, setIsOpen] = useState(false);
  const options = [
    { value: GameTypes.LONG_NUMBER, label: "Long number memory" },
    { value: GameTypes.CHIMP, label: "Chimp test" },
    { value: GameTypes.SEQUENCE, label: "Sequence memory" },
  ];

  const handleOptionClick = useCallback((value: string) => {
    setSelectedValue(value);
    onSelectChange(value);
    setIsOpen(false);
  }, []);

  const toggleDropdown = () => setIsOpen(!isOpen);

  return (
    <div className="dropdown-container">
      <div className="custom-select" onClick={toggleDropdown}>
        {selectedValue
          ? options.find((opt) => opt.value === selectedValue)?.label
          : "Select an option"}
      </div>
      {isOpen && (
        <div className="dropdown-options">
          {options.map((option) => (
            <div
              key={option.value}
              className="dropdown-option"
              onClick={() => handleOptionClick(option.value)}
            >
              {option.label}
            </div>
          ))}
        </div>
      )}
    </div>
  );
};
