import React, { useState } from 'react';
import { deleteShortUrl } from '../services/UrlService';

function DeleteShortUrlButton() {
    const [shortUrl, setShortUrl] = useState('');

    const handleClick = async () => {
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
            <input
                type="text"
                value={shortUrl}
                onChange={(e) => setShortUrl(e.target.value)}
                placeholder="Enter short URL to delete"
            />
            <button onClick={handleClick}>Delete Short URL</button>
        </div>
    );
}

export default DeleteShortUrlButton;
