-- Tabela de usuários
CREATE TABLE usuarios (
  id SERIAL PRIMARY KEY,
  nome VARCHAR(100) NOT NULL,
  email VARCHAR(100) UNIQUE NOT NULL,
  senha VARCHAR(255) NOT NULL,
  perfil VARCHAR(20) NOT NULL,
  data_cadastro TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  ativo BOOLEAN DEFAULT true
);

-- Tabela de projetos
CREATE TABLE projetos (
  id SERIAL PRIMARY KEY,
  titulo VARCHAR(200) NOT NULL,
  numero_edital VARCHAR(50),
  data_inicio DATE NOT NULL,
  data_fim DATE,
  valor_total_orcamento DECIMAL(15,2) NOT NULL,
  fk_usuario_responsavel INTEGER NOT NULL,
  criado_em TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Tabela de categorias de despesa
CREATE TABLE categorias_despesa (
  id SERIAL PRIMARY KEY,
  nome VARCHAR(50) NOT NULL,
  descricao TEXT
);

-- Tabela de naturezas de despesa
CREATE TABLE naturezas_despesa (
  id SERIAL PRIMARY KEY,
  fk_categoria INTEGER NOT NULL,
  nome VARCHAR(100) NOT NULL,
  unidade_medida VARCHAR(20)
);

-- Tabela de itens orçados
CREATE TABLE itens_orcados (
  id SERIAL PRIMARY KEY,
  fk_projeto INTEGER NOT NULL,
  fk_natureza INTEGER NOT NULL,
  descricao TEXT,
  quantidade_orcada DECIMAL(12,2) NOT NULL,
  valor_unitario_orcado DECIMAL(15,2) NOT NULL,
  valor_total_orcado DECIMAL(15,2) NOT NULL,
  criado_em TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Tabela de itens realizados
CREATE TABLE itens_realizados (
  id SERIAL PRIMARY KEY,
  fk_item_orcado INTEGER,
  fk_projeto INTEGER NOT NULL,
  data_despesa DATE NOT NULL,
  numero_documento VARCHAR(50),
  quantidade_realizada DECIMAL(12,2) NOT NULL,
  valor_unitario_realizado DECIMAL(15,2) NOT NULL,
  valor_total_realizado DECIMAL(15,2) NOT NULL,
  fornecedor VARCHAR(200),
  tipo_despesa VARCHAR(20) DEFAULT 'prevista',
  justificativa TEXT,
  observacao TEXT,
  criado_em TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  fk_usuario_registro INTEGER NOT NULL
);

-- Foreign Keys
ALTER TABLE projetos 
  ADD CONSTRAINT fk_projetos_usuario 
  FOREIGN KEY (fk_usuario_responsavel) 
  REFERENCES usuarios (id);

ALTER TABLE itens_realizados 
  ADD CONSTRAINT fk_itens_realizados_usuario 
  FOREIGN KEY (fk_usuario_registro) 
  REFERENCES usuarios (id);

ALTER TABLE itens_realizados 
  ADD CONSTRAINT fk_itens_realizados_orcado 
  FOREIGN KEY (fk_item_orcado) 
  REFERENCES itens_orcados (id);

ALTER TABLE itens_orcados 
  ADD CONSTRAINT fk_itens_orcados_projeto 
  FOREIGN KEY (fk_projeto) 
  REFERENCES projetos (id);

ALTER TABLE itens_realizados 
  ADD CONSTRAINT fk_itens_realizados_projeto 
  FOREIGN KEY (fk_projeto) 
  REFERENCES projetos (id);

ALTER TABLE naturezas_despesa 
  ADD CONSTRAINT fk_naturezas_categoria 
  FOREIGN KEY (fk_categoria) 
  REFERENCES categorias_despesa (id);

ALTER TABLE itens_orcados 
  ADD CONSTRAINT fk_itens_orcados_natureza 
  FOREIGN KEY (fk_natureza) 
  REFERENCES naturezas_despesa (id);
