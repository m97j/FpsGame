const express = require('express');
const cors = require('cors');
const bodyParser = require('body-parser');
const authRoutes = require('./routes/authRoutes');
const mongoose = require('mongoose');

const app = express();
const PORT = process.env.PORT || 5000;

const Mongo_URL = process.env.Mongo_URL

app.use(cors());
app.use(bodyParser.json());

app.use('/api', authRoutes); // /api/login, /api/signup

mongoose.connect(Mongo_URL, {
    useNewUrlParser: true,
    useUnifiedTopology: true,
})
.then(() => {
    console.log('✅ MongoDB connected');
    app.listen(PORT, () => console.log(`✅ Server running on port ${PORT}`));
})
.catch(err => console.error('❌ MongoDB connection error:', err));
