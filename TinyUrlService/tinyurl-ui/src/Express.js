const express = require('express');
const cors = require('cors');
const app = express();

var corsOptions = {
    origin: "http://localhost:3000"
};

// Allow all origins, all methods, and specific headers
app.use(cors(corsOptions));

// Other middleware and route setup
app.use(express.json());
app.use('/api', require('./routes/api')); // Example route setup

const PORT = process.env.PORT || 3000;
app.listen(PORT, () => {
    console.log(`Server listening on port ${PORT}`);
});
