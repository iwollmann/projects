const { celebrate, Segments, Joi } = require('celebrate');

const ufs = ["AC","AL","AP","AM","BA","CE","DF","ES","GO","MA","MT","MS","MG","PA","PB","PR","PE","PI","RJ","RN","RS","RO","RR","SC","SP","SE","TO"];

module.exports = {
    createOng: celebrate({
        [Segments.BODY] : Joi.object().keys({
            name: Joi.string().required(),
            email: Joi.string().required().email(),
            whatsapp: Joi.string().required().min(10).max(11),
            city: Joi.string().required(),
            uf: Joi.string().required().insensitive().uppercase().options({convert: true}).valid(...ufs),
        })
    })
}