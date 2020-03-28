const { celebrate, Segments, Joi } = require('celebrate');

module.exports = {
    showProfile: celebrate({
        [Segments.HEADERS] : Joi.object({
            authorization: Joi.string().required(),
        }).unknown(),
    }),
}