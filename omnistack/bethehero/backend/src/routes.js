const express = require('express')
const { celebrate, Segments, Joi } = require('celebrate');

const SessionController = require('./controllers/sessioncontroller');
const OngController = require('./controllers/ongcontroller');
const IncidentController = require('./controllers/incidentcontroller');
const ProfileController = require('./controllers/profilecontroller');

const validators = require('./validators');

const routes = express.Router();

routes.post('/sessions', SessionController.create);

routes.get('/ongs', OngController.index);
routes.post('/ongs', validators.createOng ,OngController.create);

routes.get('/profile', validators.showProfile , ProfileController.index);

routes.get('/incidents', IncidentController.index);
routes.post('/incidents', IncidentController.create);
routes.delete('/incidents/:id', IncidentController.delete);

module.exports = routes;