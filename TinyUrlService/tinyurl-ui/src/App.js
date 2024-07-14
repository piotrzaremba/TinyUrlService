import React from 'react';
import UrlShortener from './components/UrlShortener';
import Statistics from './components/Statistics';
import DeleteShortUrlButton from './components/DeleteShortUrlButton';

function App() {
    return (
        <div className="App">
            <h1>TinyURL POC</h1>
            <UrlShortener />
            <Statistics />
        </div>
    );
}

export default App;
