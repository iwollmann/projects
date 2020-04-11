const express = require("express");
const cors = require("cors");

const { uuid, isUuid } = require("uuidv4");

const app = express();

const isValidUuid = (request, response, next) => {
  const { id } = request.params;

  if (!isUuid(id)) return response.status(400).json({ error: 'Invalid Id.'});

  return next();
}

app.use(express.json());
app.use(cors());
app.use('/repositories/:id', isValidUuid);

const repositories = [];

app.get("/repositories", (request, response) => {
  return response.json(repositories);
});

app.post("/repositories", (request, response) => {
  const { title, url, techs } = request.body;

  const repository = {
    id : uuid(),
    title,
    url,
    techs,
    likes : 0
  }

  repositories.push(repository);

  return response.json(repository);
});

app.put("/repositories/:id", (request, response) => {
  const { id } = request.params;
  const { title, url, techs } = request.body;

  const repositoryIndex = repositories.findIndex(x=> x.id === id);

  if (repositoryIndex < 0) return response.status(400).json({ error: `respository ${id} not found!` });

  const repository = Object.assign(repositories[repositoryIndex], {
    title,
    url,
    techs
  });

  repositories[repositoryIndex] = repository;
  return response.json(repository);
});

app.delete("/repositories/:id", (request, response) => {
  const { id } = request.params;

  const repositoryIndex = repositories.findIndex(x=> x.id === id);

  if (repositoryIndex < 0) return response.status(400).json({ error: `respository ${id} not found!` });

  repositories.splice(repositoryIndex, 1);

  return response.status(204).send();
});

app.post("/repositories/:id/like", (request, response) => {
  const { id } = request.params;

  const repositoryIndex = repositories.findIndex(x=> x.id === id);

  if (repositoryIndex < 0) return response.status(400).json({ error: `respository ${id} not found!` });

  repositories[repositoryIndex].likes++;
  
  return response.json(repositories[repositoryIndex]);
});

module.exports = app;
