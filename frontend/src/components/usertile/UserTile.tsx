import React from 'react';
import './UserTile.css';

interface UserTileProps {
    children: React.ReactNode;
  }

  export const UserTile = ({ children }: UserTileProps) => {
    return (
      <div className="user-tile">
        {children}
      </div>
    );
  };
