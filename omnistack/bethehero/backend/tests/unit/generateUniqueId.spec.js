const generateUniqueId = require('../../src/utils/generateUniqueId');

describe('Generate unique ID', () => {
    it('should be able to generate an unique identifier', () => {
        const id = generateUniqueId();

        expect(id).toHaveLength(8);
    });
});