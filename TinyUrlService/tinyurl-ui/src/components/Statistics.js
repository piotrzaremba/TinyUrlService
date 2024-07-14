import React, { useEffect, useState } from 'react';
import { getStatistics, getLongUrl, deleteShortUrl } from '../services/UrlService';

function Statistics() {
    const [stats, setStats] = useState([]);

    useEffect(() => {
        const fetchData = async () => {
            const result = await getStatistics();
            setStats(result);
        };
        fetchData();
    }, []);

    const handleClick = async (shortUrl) => {
        try {
            const response = await getLongUrl(shortUrl);
            if (response.ok) {
                window.open(response.url, '_blank').focus();
            } else {
                console.error('Failed to fetch long URL');
            }
        } catch (error) {
            console.error('Error fetching long URL', error);
        }
    };

    const handleDelete = async (shortUrl) => {
        try {
            const success = await deleteShortUrl(shortUrl);
            if (success) {
                window.location.reload();
            } else {
                console.error('Failed to delete short URL:', shortUrl);
            }
        } catch (error) {
            console.error('Error deleting short URL:', error);
        }
    };

    return (
        <div>
            <h2>Statistics</h2>
            <ul>
                {Object.entries(stats).map(([shortUrl, data]) => (
                    <li key={shortUrl}>
                        <a href="#" onClick={() => handleClick(shortUrl)}>
                            {shortUrl}
                        </a>: {data.longUrl} (Clicks: {data.clickCount})
                        <button onClick={() => handleDelete(shortUrl)}>Delete</button>
                    </li>
                ))}
            </ul>
        </div>
    );
}

export default Statistics;
