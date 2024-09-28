import { useCallback, useState } from "react";
import "./Dropdown.css";

interface DropdownProps {
  onSelectChange: (value: string) => void;
}

export const Dropdown = ({ onSelectChange }: DropdownProps) => {
  const [selectedValue, setSelectedValue] = useState("");
  const options = [
    { value: "Long number memory", label: "Long number memory" },
    { value: "Chimp test", label: "Chimp test" },
    { value: "Sequence memory", label: "Sequence memory" },
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
