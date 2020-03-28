const request = require('supertest');
const app = require('../../src/app');

const connection = require('../../src/database/connection');

describe('ONG', () => {
    beforeEach(async () => {
        await connection.migrate.rollback(true);
        await connection.migrate.latest();
    });

    afterAll( async () => {
        await connection.destroy();
    });

    it('should be able to create a new ONG', async ()=> {
        const response = await request(app)
            .post('/ongs')
            .send({
                name: "APAD",
                email: "test@test.com",
                whatsapp: "2190000000",
                city: "Rio de Janeiro",
                uf: "RJ"
            });

        expect(response.body).toHaveProperty('id');
        expect(response.body.id).toHaveLength(8);
    });
});
