import { BrowserRouter, Routes, Route } from 'react-router-dom';
import './App.css'
import { Home } from './pages/Home';

export const App = () =>  {

  return (
    <BrowserRouter>
      <Routes>
        <Route path={'/'} element={<Home />}/>
      </Routes>
    </BrowserRouter>
  )
}

